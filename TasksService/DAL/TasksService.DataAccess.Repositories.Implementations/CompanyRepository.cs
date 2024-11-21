using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Rpc;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TasksService.DataAccess.Entities;
using TasksService.DataAccess.EntityFramework;
using TasksService.DataAccess.Repositories.Abstractions;
using static System.Net.Mime.MediaTypeNames;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TasksService.DataAccess.Repositories.Implementations
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IHistoryRepository _historyRepo;


        public CompanyRepository(IConfiguration configuration, IHistoryRepository historyRepo)
        {
            _configuration = configuration;
            _historyRepo = historyRepo;
        }

        public async Task<List<TasksCompany>> GetCompanies(long id)
        {
            try
            {
                var comps = new List<TasksCompany>();
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    if (id == 0)
                        comps.AddRange(await dbContext.Companies.ToListAsync());
                    else
                    {
                        var res = await dbContext.Companies.Where(u => u.Id == id).ToListAsync();
                        if (res.Any())
                            comps.Add(res.First());
                    }
                    return comps;
                }
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке получения приоритета", ex.StackTrace ?? "");
            }
        }

        public async Task<long> CreateCompany(Guid creatorId, string name, string description)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    var newItem = new TasksCompany() { Name = name, Description = description };
                    dbContext.Companies.Add(newItem);
                    await dbContext.SaveChangesAsync();
                    RabbitMQService.SendToRabbit(newItem, LoggerService.ELogAction.LaCreate, creatorId.ToString());
                    return newItem.Id;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ошибка при выполнении запроса: {Request}", request);
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке создания приоритета", ex.StackTrace ?? "");
            }

        }
        public async Task<bool> ModifyCompany(Guid userId, long changeFlags, long id, string name, string description)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    var company = new TasksCompany(){ Id = id, Name = name, Description = description};
                    dbContext.Companies.Attach(company);
                    dbContext.Entry(company).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();
                    RabbitMQService.SendToRabbit(company, LoggerService.ELogAction.LaUpdate, userId.ToString());
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке изменения компании", ex.StackTrace ?? "");
            }

        }

        public async Task<bool> DeleteCompany(Guid userId, long companyId)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    var foundComp = dbContext.Companies.FirstOrDefault(u => u.Id == companyId);
                    if (null == foundComp)
                        return false;

                    dbContext.Companies.Remove(foundComp);
                    await dbContext.SaveChangesAsync();
                    RabbitMQService.SendToRabbit(foundComp, LoggerService.ELogAction.LaDelete, userId.ToString());
                    return true;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ошибка при выполнении запроса: {Request}", request);
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке удаления компании", ex.StackTrace ?? "");
            }
        }

    }
}
