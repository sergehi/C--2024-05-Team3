using System.Collections.Generic;
using System.Threading.Tasks;
using AuthorizationService.Core.Entities;

namespace AuthorizationService.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
    }
}
