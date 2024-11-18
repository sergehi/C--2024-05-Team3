using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Rpc;
using Grpc.Core;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using TasksService.DataAccess.Entities;
using TasksService.DataAccess.EntityFramework;
using TasksService.DataAccess.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace TasksService.DataAccess.Repositories.Implementations
{

    public class EmployeesRepository : IEmployeesRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IHistoryRepository _historyRepo;

        public EmployeesRepository(IConfiguration configuration, IHistoryRepository historyRepo)
        {
            _configuration = configuration;
            _historyRepo = historyRepo;
        }

        public async Task<bool> AddEmployee(Guid creatorId, long companyId, Guid newUserId)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    var newItem = new CompanyEmployee() { CompanyId = companyId, EmployeeId = newUserId};
                    var found = dbContext.CompanyEmployees.FirstOrDefault(x => x.EmployeeId == newUserId && x.CompanyId == companyId);
                    if (found == null)
                        dbContext.CompanyEmployees.Add(newItem);  

                    await dbContext.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ошибка при выполнении запроса: {Request}", request);
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке создания пользователя", ex.StackTrace);
            }
        }

        public async Task<List<Guid>> GetEmployees(long companyId)
        {
            try
            {
                var emps = new List<Guid>();
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    if (companyId == 0)
                        emps.AddRange(await dbContext.CompanyEmployees.Select(e => e.EmployeeId).ToListAsync());
                    else
                    {
                        var res = await dbContext.CompanyEmployees.Where(u => u.CompanyId == companyId).Select(e => e.EmployeeId).ToListAsync();
                        if (res.Any())
                            emps.Add(res.First());
                    }
                    return emps;
                }
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке получения списка пользователей", ex.StackTrace ?? "");
            }

        }

        public async Task<bool> RemoveEmployee(Guid deleterId, long companyId, Guid userToDelId)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    var foundEmpl = dbContext.CompanyEmployees.FirstOrDefault(u => u.CompanyId == companyId && u.EmployeeId == userToDelId);
                    if (null == foundEmpl)
                        return false;

                    dbContext.CompanyEmployees.Remove(foundEmpl);
                    await dbContext.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ошибка при выполнении запроса: {Request}", request);
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке удаления пользователя", ex.StackTrace ?? "");
            }
        }
    }
}
