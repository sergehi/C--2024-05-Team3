using AuthorizationService.Core.Entities;

namespace AuthorizationService.Core.Interfaces
{
    public interface IUserRepository
    {
        Task CreateAsync(User user);
        Task<User?> FindByUsernameAsync(string username);
        Task UpdateAsync(User user);
        Task<User?> FindByIdAsync(Guid id);
    }
}