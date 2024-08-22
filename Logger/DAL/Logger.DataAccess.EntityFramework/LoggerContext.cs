using Logger.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Logger.DataAccess.EntityFramework
{
    public class LoggerContext : DbContext
    {
        private IConfiguration _configuration;

        public DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Log>()
                .HasIndex(l => l.Time);

            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration["ConnectionStrings:LoggerDB"]);
        }

        public LoggerContext(IConfiguration configuration)
        {
            _configuration = configuration;
            Database.EnsureCreated();
        }
    }
}
