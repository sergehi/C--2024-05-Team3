using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksService.DataAccess.Repositories.Abstractions
{
    public interface IEmployeesRepository
    {
        public Task<List<long>> GetEmployees(long companyId);
        public Task<bool> AddEmployee(long creatorId, long companyId, long newUserId);
        public Task<bool> RemoveEmployee(long deleterId, long companyId, long userToDelId);
    }
}
