using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Common.Rpc;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TasksService.DataAccess.Entities;
using TasksService.DataAccess.EntityFramework;
using TasksService.DataAccess.Repositories.Abstractions;
using static NpgsqlTypes.NpgsqlTsQuery;

namespace TasksService.DataAccess.Repositories.Implementations
{
    public class HistoryRepository : IHistoryRepository
    {
        private readonly IConfiguration _configuration;
        public HistoryRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<TaskHistory>> GetTaskHistory(long TaskId)
        {
            try
            {
                var prjs = new List<TaskHistory>();
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    if (TaskId != 0)
                        return await dbContext.TaskHistories.Where(e => e.TaskId == TaskId).ToListAsync();
                    throw new Exception("Не указан идентификатор задачи");
                }
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке получения истории изменений задачи", ex.StackTrace ?? "");
            }
        }

        public async Task<bool> RegisterTaskUrgencyChanged(Guid UserId, long TaskId, string OldValue, string NewValue)
        {
            return await registerTaskChange(UserId, TaskId, 0, ETaskModifyFlags.TMF_MODIFY_URGENCY, OldValue, NewValue);
        }
        public async Task<bool> RegisterTaskTypeChanged(Guid UserId, long TaskId, string OldValue, string NewValue)
        {
            return await registerTaskChange(UserId, TaskId, 0, ETaskModifyFlags.TMF_MODIFY_TYPE, OldValue, NewValue);
        }

        public async Task<bool> RegisterTaskStateChanged(Guid UserId, long TaskId, string OldValue, string NewValue)
        {
            return await registerTaskChange(UserId, TaskId, 0, ETaskModifyFlags.TMF_MODIFY_STATE, OldValue, NewValue);
        }

        public async Task<bool> RegisterTaskNameChanged(Guid UserId, long TaskId, string OldValue, string NewValue)
        {
            return await registerTaskChange(UserId, TaskId, 0, ETaskModifyFlags.TMF_MODIFY_NAME, OldValue, NewValue);
        }

        public async Task<bool> RegisterTaskDescriptionChanged(Guid UserId, long TaskId, string OldValue, string NewValue)
        {
            return await registerTaskChange(UserId, TaskId, 0, ETaskModifyFlags.TMF_MODIFY_DESCRIPTION, OldValue, NewValue);
        }

        public async Task<bool> RegisterTaskDeadlineChanged(Guid UserId, long TaskId, long NodeId, string OldValue, string NewValue)
        {
            return await registerTaskChange(UserId, TaskId, NodeId, ETaskModifyFlags.TMF_MODIFY_DEADLINE, OldValue, NewValue);
        }

        public async Task<bool> RegisterTaskDoersAppointed(Guid UserId, long TaskId, long NodeId, string OldValue, string NewValue)
        {
            return await registerTaskChange(UserId, TaskId, NodeId, ETaskModifyFlags.TMF_REC_DOERS, OldValue, NewValue);
        }

        public async Task<bool> RegisterDeleteTask(Guid UserId, long TaskId, string OldValue, string NewValue)
        {
            return await registerTaskChange(UserId, TaskId, 0, ETaskModifyFlags.TMF_REC_DELETE, OldValue, NewValue);
        }

        public async Task<bool> RegisterCreateTask(Guid UserId, Entities.Task task, string NewValue)
        {
            return await registerTaskChange(UserId, task, 0, ETaskModifyFlags.TMF_REC_CREATE, string.Empty, NewValue);
        }

        public async Task<bool> registerTaskChange(Guid UserId, Entities.Task task, long NodeId, ETaskModifyFlags ChangeFlag, string OldValue, string NewValue)
        {
            try
            {
                if (ChangeFlag == ETaskModifyFlags.TMF_NONE)
                    return true;

                var taskHistory = new TaskHistory()
                {
                    TaskId = task.Id,
                    //Task = task,
                    NodeId = NodeId,
                    UserId = UserId,
                    ActionValue = NewValue,
                    OldValue = OldValue
                };
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    var action = await dbContext.TaskActions.FirstOrDefaultAsync(t => t.ActionId == (long)ChangeFlag);
                    if (action == null)
                        throw new Exception($"Не найдено описание действия \"{ChangeFlag}\" в базе данных");

                    taskHistory.ActionId = action.ActionId;
                    dbContext.TaskHistories.Add(taskHistory);
                    await dbContext.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке регистрации действий для истории", ex.StackTrace ?? "");
            }
        }

        public async Task<bool> registerTaskChange(Guid UserId, long TaskId, long NodeId, ETaskModifyFlags ChangeFlag, string OldValue, string NewValue)
        {
            try
            {
                if (ChangeFlag == ETaskModifyFlags.TMF_NONE)
                    return true;

                var taskHistory = new TaskHistory()
                {
                    TaskId = TaskId,
                    NodeId = NodeId,
                    UserId = UserId,
                    ActionValue = NewValue,
                    OldValue = OldValue
                };
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    var action = await dbContext.TaskActions.FirstOrDefaultAsync(t => t.ActionId == (long)ChangeFlag);
                    if (action == null)
                        throw new Exception($"Не найдено описание действия \"{ChangeFlag}\" в базе данных");

                    taskHistory.ActionId = action.ActionId;
                    dbContext.TaskHistories.Add(taskHistory);
                    await dbContext.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке регистрации действий для истории", ex.StackTrace ?? "");
            }
        }

        public async Task<List<string>> GetCompanyEmployees(Guid UserId, long CompanyId)
        {
            try
            { 
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    var res = await dbContext.CompanyEmployees.Where(u => u.CompanyId == CompanyId && u.EmployeeId == UserId).ToListAsync();
                    if (res.Any())
                        return await dbContext.CompanyEmployees
                            .Where(u => u.CompanyId == CompanyId)
                            .Select(x => x.EmployeeId.ToString())
                            .ToListAsync();
                }
                return new List<string>();
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке получения работников", ex.StackTrace ?? "");
            }
        }
        public async Task<List<string>> GetProjectEmployees(Guid UserId, long ProjectId)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    var res = await dbContext.CompanyProjects.Where(u => u.Id  == ProjectId ).ToListAsync();
                    if (res.Any())
                        return await dbContext.CompanyEmployees
                            .Where(u => u.CompanyId == res.First().CompanyId)
                            .Select(x => x.EmployeeId.ToString())
                            .ToListAsync();
                }
                return new List<string>();
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке получения работников", ex.StackTrace ?? "");
            }
        }


    }
}
