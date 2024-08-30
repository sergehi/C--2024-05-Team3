using Logger.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logger.DataAccess.EntityFramework
{
    public class LoggerContext : DbContext
    {
        //private readonly IConfiguration _configuration;

        public DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Log>()
                .HasIndex(l => l.Time);
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseNpgsql(_configuration["ConnectionStrings:LoggerDB"]);
        //}

        //public LoggerContext(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //    Database.EnsureCreated();
        //}

        public LoggerContext(DbContextOptions<LoggerContext> options) : base(options) {}
    }
}
