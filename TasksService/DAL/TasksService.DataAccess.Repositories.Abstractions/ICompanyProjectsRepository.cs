using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksService.DataAccess.Repositories.Abstractions
{
    public interface ICompanyProjectsRepository
    {
        public Task<List<Entities.CompanyProject>> GetCompanyProjects(long companyId, long projectId);
        public Task<long> CreateCompanyProject(long userId, Entities.CompanyProject companyProject);
        public Task<bool> ModifyCompanyProject(long userId, long changeFlags, Entities.CompanyProject project);
        public Task<bool> DeleteCompanyProject(long userId, long projectId);
    }
}
