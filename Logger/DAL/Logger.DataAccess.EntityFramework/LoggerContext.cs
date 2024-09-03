using Logger.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Logger.DataAccess.EntityFramework
{
    public class LoggerContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Log>()
                .HasIndex(l => l.Time);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if RELEASE
            var connstr = _configuration.GetConnectionString("LoggerDB");
            optionsBuilder.UseNpgsql(connstr);
#else
            optionsBuilder.UseNpgsql(_configuration["ConnectionStrings:LoggerDB"]);
#endif

        }

        public LoggerContext(IConfiguration configuration)
        {
            _configuration = configuration;
            //Database.EnsureCreated();
        }

        //public LoggerContext(DbContextOptions<LoggerContext> options) : base(options) {}
    }
}
