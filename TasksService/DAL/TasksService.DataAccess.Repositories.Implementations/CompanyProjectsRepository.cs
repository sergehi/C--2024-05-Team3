using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Common.Rpc;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TasksService.DataAccess.Entities;
using TasksService.DataAccess.EntityFramework;
using TasksService.DataAccess.Repositories.Abstractions;

namespace TasksService.DataAccess.Repositories.Implementations
{
    public class CompanyProjectsRepository : ICompanyProjectsRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IHistoryRepository _historyRepo;

        public CompanyProjectsRepository(IConfiguration configuration, IHistoryRepository historyRepo)
        {
            _configuration = configuration;
            _historyRepo = historyRepo;
        }

        public async Task<List<CompanyProject>> GetCompanyProjects(long companyId, long projectId)
        {
            try
            {
                var prjs = new List<CompanyProject>();
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    if (companyId == 0)
                    {
                        if (projectId == 0)
                            prjs.AddRange(await dbContext.CompanyProjects.ToListAsync());
                        else
                            prjs.AddRange(await dbContext.CompanyProjects.Where(e => e.Id == projectId).ToListAsync());
                    }
                    else
                    {
                        if (projectId == 0)
                            prjs.AddRange(await dbContext.CompanyProjects.Where(e =>e.CompanyId == companyId).ToListAsync());
                    }
                    return prjs;
                }
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке получения списка проектов", ex.StackTrace ?? "");
            }
        }

        public async Task<bool> ModifyCompanyProject(Guid userId, long changeFlags, CompanyProject project)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    dbContext.CompanyProjects.Attach(project);
                    dbContext.Entry(project).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке изменения проекта", ex.StackTrace ?? "");
            }
        }

        public async Task<long> CreateCompanyProject(Guid userId, CompanyProject companyProject)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    dbContext.CompanyProjects.Add(companyProject);
                    await dbContext.SaveChangesAsync();
                    return companyProject.Id;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ошибка при выполнении запроса: {Request}", request);
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке создания проекта", ex.StackTrace??"");
            }

        }

        public async Task<bool> DeleteCompanyProject(Guid userId, long projectId)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    var found = dbContext.CompanyProjects.FirstOrDefault(u => u.Id == projectId);
                    if (null == found)
                        return false;

                    dbContext.CompanyProjects.Remove(found);
                    await dbContext.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ошибка при выполнении запроса: {Request}", request);
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке удаления проекта", ex.StackTrace ?? "");
            }
        }


    }
}
