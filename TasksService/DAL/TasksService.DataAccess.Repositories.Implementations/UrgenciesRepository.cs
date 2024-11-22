using System;
using System.Collections.Generic;
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
using static NpgsqlTypes.NpgsqlTsQuery;

namespace TasksService.DataAccess.Repositories.Implementations
{
    public class UrgenciesRepository : IUrgenciesRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IHistoryRepository _historyRepo;

        public UrgenciesRepository(IConfiguration configuration, IHistoryRepository historyRepo) 
        { 
            _configuration = configuration;
            _historyRepo = historyRepo;
        }

        public async Task<long> CreateUrgency(Guid userId, string name, string description)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    var newItem = new Urgency() { Name = name, Description = description };
                    dbContext.Urgencies.Add(newItem);
                    await dbContext.SaveChangesAsync();
                    RabbitMQService.SendToRabbit(newItem, LoggerService.ELogAction.LaCreate, userId.ToString(), new List<string>() { Guid.Empty.ToString() });
                    return newItem.Id;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ошибка при выполнении запроса: {Request}", request);
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке создания приоритета", ex.StackTrace);
            }
        }

        public async Task<bool> DeleteUrgency(Guid userId, long urgencyId)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    var foundUrgcy = dbContext.Urgencies.FirstOrDefault(u=>u.Id == urgencyId);
                    if (null == foundUrgcy)
                        return false;

                    dbContext.Urgencies.Remove(foundUrgcy);
                    await dbContext.SaveChangesAsync();
                    RabbitMQService.SendToRabbit(foundUrgcy, LoggerService.ELogAction.LaDelete, userId.ToString(), new List<string>() { Guid.Empty.ToString() });
                    return true;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ошибка при выполнении запроса: {Request}", request);
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке удаления приоритета", ex.StackTrace??"");
            }
        }

        public async Task<List<Urgency>> GetUrgencies(long id)
        {
            try
            {
                var urgss = new List<Urgency>();
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    if (id == 0)
                    {
                        var urgs = await dbContext.Urgencies.ToListAsync();
                        urgss.AddRange(urgs);
                    }
                    else
                    {
                        var res = await dbContext.Urgencies.Where(u => u.Id == id).ToListAsync();
                        if (res.Any())
                            urgss.Add(res.First());
                    }
                    return urgss;
                }
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке получения приоритета", ex.StackTrace ?? "");
            }
        }

        public async Task<bool> ModifyUrgency(Guid userId, long changeFlags, Urgency urgency)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    dbContext.Urgencies.Attach(urgency);
                    dbContext.Entry(urgency).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();
                    RabbitMQService.SendToRabbit(urgency, LoggerService.ELogAction.LaUpdate, userId.ToString(), new List<string>() { Guid.Empty.ToString() });

                }
                return true;
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке изменения приоритета", ex.StackTrace ?? "");
            }
        }
    }
}
