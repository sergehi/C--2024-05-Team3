using Logger.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logger.DataAccess.EntityFramework
{
    public class LoggerContext : DbContext
    {
        public DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Log>()
                .HasIndex(l => l.Time);

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server=31.128.43.53;Port=5432;Username=postgres;Database=LoggerService;Password=jomjcpFnXs");
        }

        public LoggerContext()
        {
            Database.EnsureCreated();
        }
    }
}
