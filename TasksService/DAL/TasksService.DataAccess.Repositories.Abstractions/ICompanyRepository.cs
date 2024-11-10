using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksService.DataAccess.Entities;

namespace TasksService.DataAccess.Repositories.Abstractions
{
    public interface ICompanyRepository
    {
        Task<List<TasksCompany>> GetCompanies(long id);
        Task<long> CreateCompany(long creatorId, string name, string description);
        Task<bool> ModifyCompany(long userId, long changeFlags, long id, string name, string description);
        Task<bool> DeleteCompany(long userId, long companyId);
    }
}
