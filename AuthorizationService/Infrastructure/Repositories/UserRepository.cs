using AuthorizationService.Core.Interfaces;
using AuthorizationService.Core.Entities;
using AuthorizationService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataBaseContext _dataBaseContext;

        public UserRepository(DataBaseContext dataBaseContext)
        {
            _dataBaseContext = dataBaseContext;
        }

        public async Task CreateAsync(User user)
        {
            await _dataBaseContext.Users.AddAsync(user);
            await _dataBaseContext.SaveChangesAsync();
        }

        public async Task<User?> FindByUsernameAsync(string username)
        {
            return await _dataBaseContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> FindByIdAsync(Guid id)
        {
            return await _dataBaseContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task UpdateAsync(User user)
        {
            _dataBaseContext.Users.Update(user);
            await _dataBaseContext.SaveChangesAsync();
        }
    }
}