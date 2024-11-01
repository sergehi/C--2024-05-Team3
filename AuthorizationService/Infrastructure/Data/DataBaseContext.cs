using Microsoft.EntityFrameworkCore;
using AuthorizationService.Core.Entities;

namespace AuthorizationService.Infrastructure.Data
{
    public class DataBaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<TokenSettings> TokensSettings { get; set; }

        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(u => u.RefreshTokenExpiryTime)
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        }
    }
}