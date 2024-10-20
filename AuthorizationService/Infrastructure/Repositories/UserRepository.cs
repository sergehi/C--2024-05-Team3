using AuthorizationService.Core.Entities;
using AuthorizationService.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using AuthorizationService.Infrastructure.Data;

namespace AuthorizationService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _authDbContext;

        public UserRepository(AuthDbContext authDbContext)
        {
            _authDbContext = authDbContext;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _authDbContext.Users.FindAsync(id);
        }
    }
}
