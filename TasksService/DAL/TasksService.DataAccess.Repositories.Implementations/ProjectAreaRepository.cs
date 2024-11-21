using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Common;
using Common.Rpc;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TasksService.DataAccess.Entities;
using TasksService.DataAccess.EntityFramework;
using TasksService.DataAccess.Repositories.Abstractions;

namespace TasksService.DataAccess.Repositories.Implementations
{
    public class ProjectAreaRepository : IProjectAreaRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IHistoryRepository _historyRepo;

        public ProjectAreaRepository(IConfiguration configuration, IHistoryRepository historyRepo)
        {
            _configuration = configuration; 
            _historyRepo = historyRepo;
        }

        public async Task<List<ProjectArea>> GetProjectAreas(long projectId, long areaId)
        {
            try
            {
                var areas = new List<ProjectArea>();
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    if (projectId == 0)
                    {
                        if (areaId != 0)
                            areas.AddRange(await dbContext.ProjectAreas.Where(x => x.Id == areaId).ToListAsync());
                        else
                            areas.AddRange(await dbContext.ProjectAreas.ToListAsync());
                    }
                    else
                    {
                        if (areaId != 0)
                            areas.AddRange(await dbContext.ProjectAreas.Where(x => x.Id == areaId && x.ProjectId == projectId).ToListAsync());
                        else
                            areas.AddRange(await dbContext.ProjectAreas.Where(x => x.ProjectId == projectId).ToListAsync());
                    }
                    return areas;
                }
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке получения направления", ex.StackTrace ?? "");
            }
        }

        public async Task<bool> ModifyProjectArea(Guid userId, long changeFlags, ProjectArea area)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    dbContext.ProjectAreas.Attach(area);
                    dbContext.Entry(area).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();
                }
                RabbitMQService.SendToRabbit(area, LoggerService.ELogAction.LaUpdate, userId.ToString(), await _historyRepo.GetProjectEmployees(userId, area.ProjectId));
                return true;
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке изменения направления", ex.StackTrace ?? "");
            }
        }

        public async Task<long> CreateProjectArea(Guid userId, ProjectArea area)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    dbContext.ProjectAreas.Add(area);
                    await dbContext.SaveChangesAsync();
                    RabbitMQService.SendToRabbit(area, LoggerService.ELogAction.LaCreate, userId.ToString(), await _historyRepo.GetProjectEmployees(userId, area.ProjectId));
                    return area.Id;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ошибка при выполнении запроса: {Request}", request);
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке создания направления", ex.StackTrace?? "");
            }

        }

        public async Task<bool> DeleteProjectArea(Guid userId, long areaId)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    var found = dbContext.ProjectAreas.FirstOrDefault(u => u.Id == areaId);
                    if (null == found)
                        return false;

                    dbContext.ProjectAreas.Remove(found);
                    await dbContext.SaveChangesAsync();
                    RabbitMQService.SendToRabbit(found, LoggerService.ELogAction.LaDelete, userId.ToString(), await _historyRepo.GetProjectEmployees(userId, found.ProjectId));
                    return true;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ошибка при выполнении запроса: {Request}", request);
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке удаления направления", ex.StackTrace ?? "");
            }
        }

    }
}
