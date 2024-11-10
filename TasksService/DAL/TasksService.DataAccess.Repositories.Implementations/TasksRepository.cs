﻿using Grpc.Core;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using TasksService.DataAccess.Entities;
using TasksService.DataAccess.EntityFramework;
using TasksService.DataAccess.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using Common.Rpc;
using System.ComponentModel.Design;
using static NpgsqlTypes.NpgsqlTsQuery;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;

namespace TasksService.DataAccess.Repositories.Implementations
{
    public class TasksRepository : ITasksRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IHistoryRepository _historyRepo;
        public TasksRepository(IConfiguration configuration, IHistoryRepository historyRepo)
        {
            _configuration = configuration;
            _historyRepo = historyRepo;
        }

        public async Task<long> CreateTask(long userId, Entities.Task taskToCreate)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    using (var transaction = dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            // GetTemplate
                            var template = await dbContext.WfdefinitionsTempls.FirstOrDefaultAsync(x => x.Id == taskToCreate.TemplateId);
                            if (null == template)
                                throw new Exception($"Не найден шаблон с идентификатором {taskToCreate.TemplateId}");
                            
                            // Create task from template
                            taskToCreate.CreatorId = userId;
                            dbContext.Tasks.Add(taskToCreate);
                            await dbContext.SaveChangesAsync();
                            // Copy nodes from temlate to task
                            foreach (var templNode in template.WfnodesTempls)
                            {
                                var newNode = new TaskNode() 
                                {  
                                    Name = templNode.Name 
                                    , Description = templNode.Description
                                    , TaskId = taskToCreate.Id
                                    , Terminating = templNode.Terminating
                                    , TemplateId = template.Id

                                };
                                dbContext.TaskNodes.Add(newNode);
                            }
                            await dbContext.SaveChangesAsync();
                            // Copy rdges from temlate to task
                            foreach (var templNode in template.WfnodesTempls)
                            {
                                foreach (var templEdge in templNode.WfedgesTemplNodeFromNavigations)
                                {
                                    var newEdge = new TaskEdge();
                                    newEdge.Name = templEdge.Name;
                                    var tn_from = dbContext.TaskNodes.FirstOrDefault(x => x.TemplateId == templEdge.Id);
                                    if (tn_from == null)
                                        throw new Exception($"Не найден начальный узел {templEdge.Name} при построении грани");
                                    newEdge.NodeFrom = tn_from.Id;

                                    var tn_to = dbContext.TaskNodes.FirstOrDefault(x => x.TemplateId == templEdge.Id);
                                    if (tn_to == null)
                                        throw new Exception($"Не найден конечный узел {templEdge.Name} при построении грани");
                                    newEdge.NodeTo = tn_to.Id;
                                    dbContext.TaskEdges.Add(newEdge);
                                }
                            }
                            await dbContext.SaveChangesAsync();
                            await _historyRepo.RegisterCreateTask(userId, taskToCreate.Id, taskToCreate.Name);
                            return taskToCreate.Id;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                        finally
                        {
                            transaction.Commit();
                        }
                    }
                }
            }
            catch (DbUpdateConcurrencyException cex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, cex.Message), "Произошла ошибка при одновременной попытке создания задачи", cex.StackTrace ?? "");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ошибка при выполнении запроса: {Request}", request);
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке создания задачи", ex.StackTrace ?? "");
            }
        }

        public async Task<bool> DeleteTask(long userId, long taskId)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    using (var transaction = dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            var task = await dbContext.Tasks.FirstOrDefaultAsync(x=>x.Id == taskId);
                            if (null == task)
                                throw new Exception($"Не найдена задача с идентификатором {taskId}");

                            dbContext.Tasks.Remove(task);
                            await dbContext.SaveChangesAsync();
                            await _historyRepo.RegisterDeleteTask(userId, taskId, task.Name, "");
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                        finally
                        {
                            transaction.Commit();
                        }
                    }
                }
            }
            catch (DbUpdateConcurrencyException cex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, cex.Message), "Произошла ошибка при одновременной попытке удаления задачи", cex.StackTrace ?? "");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ошибка при выполнении запроса: {Request}", request);
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке удаления задачи", ex.StackTrace ?? "");
            }
        }

        public async  Task<TaskNode> GetNode(long id)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    var node = await dbContext.TaskNodes.FirstOrDefaultAsync(x=>x.Id == id);
                    if (null == node)
                        throw new Exception($"Узел с идентификатором {id} не найден.");

                    return node;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ошибка при выполнении запроса: {Request}", request);
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке удаления задачи", ex.StackTrace ?? "");
            }
        }

        public async Task<List<TaskEdge>> GetFromNodeTransitions(long nodeId)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    var node = await dbContext.TaskNodes.FirstOrDefaultAsync(x => x.Id == nodeId);
                    if (null == node)
                        throw new Exception($"Узел с идентификатором {nodeId} не найден.");

                    return node.TaskEdgeNodeFromNavigations.ToList();
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ошибка при выполнении запроса: {Request}", request);
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке получить переходы из узла", ex.StackTrace ?? "");
            }
        }
        public async Task<List<TaskEdge>> GetToNodeTransitions(long nodeId)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    var node = await dbContext.TaskNodes.FirstOrDefaultAsync(x => x.Id == nodeId);
                    if (null == node)
                        throw new Exception($"Узел с идентификатором {nodeId} не найден.");

                    return node.TaskEdgeNodeToNavigations.ToList();
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ошибка при выполнении запроса: {Request}", request);
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке получить переходы в узлел", ex.StackTrace ?? "");
            }
        }

        public async  Task<Entities.Task> GetTask(long taskId)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    var task = await dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == taskId);
                    if (null == task)
                        throw new Exception($"Задача с идентификатором {taskId} не найдена.");

                    return task;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ошибка при выполнении запроса: {Request}", request);
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке получить задачу по идентификатору", ex.StackTrace ?? "");
            }
        }

        public async  Task<List<Entities.Task>> GetTasksList(long userId, long companyId, long projectId, long areaId)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    // Получаем список задач, фильтруя по заданным параметрам
                    // Начинаем запрос к таблице задач
                    var query = dbContext.Tasks.AsQueryable();

                    // Фильтруем по companyId, если он не равен 0
                    if (companyId != 0)
                    {
                        query = query.Where(t => t.CompanyId == companyId);
                    }

                    // Фильтруем по projectId, если он не равен 0
                    if (projectId != 0)
                    {
                        query = query.Where(t => t.ProjectId == projectId);
                    }

                    // Фильтруем по areaId, если он не равен 0
                    if (areaId != 0)
                    {
                        query = query.Where(t => t.AreaId == areaId);
                    }
                    // Включаем связанные данные, если это необходимо
                    query = query.Include(t => t.Area)
                                 .Include(t => t.Project)
                                 .Include(t => t.Company);
                    // Выполняем запрос и получаем список задач
                    var tasks = await query.ToListAsync();
                    return tasks;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ошибка при выполнении запроса: {Request}", request);
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке получить список задач", ex.StackTrace ?? "");
            }


        }

        public async Task<bool> ModifyTaskUrgency(long userId, long taskId, long urgId)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    using (var transaction = dbContext.Database.BeginTransaction())
                    {
                        var task = await dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == taskId);
                        if (null == task)
                            throw new Exception($"Задача с идентификатором {taskId} не найдена.");

                        var oldValue = task.Urgency;
                        task.Urgency = urgId;
                        await dbContext.SaveChangesAsync();
                        await _historyRepo.RegisterTaskUrgencyChanged(userId, taskId, oldValue.ToString(), urgId.ToString());
                        return true;
                    }
                }
            }
            catch (DbUpdateConcurrencyException cex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, cex.Message), "Произошла ошибка при одновременной попытке удаления задачи", cex.StackTrace ?? "");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ошибка при выполнении запроса: {Request}", request);
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке получить задачу по идентификатору", ex.StackTrace ?? "");
            }
        }

        public async  Task<bool> ModifyTaskState(long userId, long taskId, long toNodeId)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    using (var transaction = dbContext.Database.BeginTransaction())
                    {
                        var task = await dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == taskId);
                        if (null == task)
                            throw new Exception($"Задача с идентификатором {taskId} не найдена.");

                        var oldVal = task.CurrentNode;
                        task.CurrentNode = toNodeId;
                        await dbContext.SaveChangesAsync();
                        await _historyRepo.RegisterTaskTypeChanged(userId, taskId, oldVal.ToString(), task.CurrentNode.ToString());
                        return true;
                    }
                }
            }
            catch (DbUpdateConcurrencyException cex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, cex.Message), "Произошла ошибка при одновременной попытке изменения состояния задачи", cex.StackTrace ?? "");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ошибка при выполнении запроса: {Request}", request);
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при одновременной попытке изменения состояния задачи", ex.StackTrace ?? "");
            }
        }

        public async  Task<bool> ModifyTaskName(long userId, long taskId, string newName)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    using (var transaction = dbContext.Database.BeginTransaction())
                    {
                        var task = await dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == taskId);
                        if (null == task)
                            throw new Exception($"Задача с идентификатором {taskId} не найдена.");

                        var oldVal = task.Name;
                        task.Name = newName;
                        await dbContext.SaveChangesAsync();
                        await _historyRepo.RegisterTaskTypeChanged(userId, taskId, oldVal, task.Name);
                        return true;
                    }
                }
            }
            catch (DbUpdateConcurrencyException cex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, cex.Message), "Произошла ошибка при одновременной попытке изменения имени задачи", cex.StackTrace ?? "");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ошибка при выполнении запроса: {Request}", request);
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при одновременной попытке изменения имени задачи", ex.StackTrace ?? "");
            }
        }

        public async  Task<bool> ModifyTaskDescription(long userId, long taskId, string newDescription)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    using (var transaction = dbContext.Database.BeginTransaction())
                    {
                        var task = await dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == taskId);
                        if (null == task)
                            throw new Exception($"Задача с идентификатором {taskId} не найдена.");

                        var oldVal = task.Description;
                        task.Description = newDescription;
                        await dbContext.SaveChangesAsync();
                        await _historyRepo.RegisterTaskTypeChanged(userId, taskId, oldVal?? "", task.Description);
                        return true;
                    }
                }
            }
            catch (DbUpdateConcurrencyException cex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, cex.Message), "Произошла ошибка при одновременной попытке изменения описания задачи", cex.StackTrace ?? "");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ошибка при выполнении запроса: {Request}", request);
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при одновременной попытке изменения описания задачи", ex.StackTrace ?? "");
            }
        }

        public async  Task<bool> ModifyTaskNodeDeadline(long userId, long taskId, long nodeId, DateTime newDeadline)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    using (var transaction = dbContext.Database.BeginTransaction())
                    {
                        var task = await dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == taskId);
                        if (null == task)
                            throw new Exception($"Задача с идентификатором {taskId} не найдена.");

                        var oldVal = task.DeadlineDate;
                        task.DeadlineDate = newDeadline;
                        await dbContext.SaveChangesAsync();
                        await _historyRepo.RegisterTaskTypeChanged(userId, taskId, oldVal.ToString() ??"", task.DeadlineDate.ToString()??"");
                        return true;
                    }
                }
            }
            catch (DbUpdateConcurrencyException cex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, cex.Message), "Произошла ошибка при одновременной попытке изменения даты выполнения задачи", cex.StackTrace ?? "");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ошибка при выполнении запроса: {Request}", request);
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при одновременной попытке изменения даты выполнения задачи", ex.StackTrace ?? "");
            }
        }

        public async Task<bool> AppointNodeDoers(long userId, long nodeId, List<long> list)
        {
            try
            {
                TaskNode? node = null;
                string oldValue = "";
                string newValue = "";
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    using (var transaction = dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            node = dbContext.TaskNodes.FirstOrDefault(x => x.Id == nodeId);
                            if (null != node)
                            {
                                oldValue = string.Join(",", node.TaskDoers.Select(x => x.EmpoyeeId.ToString()).ToList());
                                foreach (var user in list)
                                    if (node.TaskDoers.Count(x => x.EmpoyeeId == user) == 0)
                                        node.TaskDoers.Add(new TaskDoer() { NodeId = node.Id, EmpoyeeId = user });
                                await dbContext.SaveChangesAsync();
                                newValue = string.Join(",", node.TaskDoers.Select(x => x.EmpoyeeId.ToString()).ToList());
                            }
                            else
                                return false;

                            await _historyRepo.RegisterTaskDoersAppointed(userId, node.TaskId, nodeId, oldValue, newValue);
                            return true;
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                        finally
                        {
                            transaction.Commit();
                        }
                    }
                }
            }
            catch (DbUpdateConcurrencyException cex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, cex.Message), "Произошла ошибка при одновременной попытке назначения исполнителей задачи", cex.StackTrace ?? "");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ошибка при выполнении запроса: {Request}", request);
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке назначения исполнителей задачи", ex.StackTrace ?? "");
            }

        }

    }

}