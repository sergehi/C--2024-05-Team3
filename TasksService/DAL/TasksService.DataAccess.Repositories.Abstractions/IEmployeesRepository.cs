using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksService.DataAccess.Repositories.Abstractions
{
    public interface IEmployeesRepository
    {
        public Task<List<Guid>> GetEmployees(long companyId);
        public Task<bool> AddEmployee(Guid creatorId, long companyId, Guid newUserId);
        public Task<bool> RemoveEmployee(Guid deleterId, long companyId, Guid userToDelId);
    }
}
