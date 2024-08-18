using ChatService.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Infrastructure.EntityFramework;

public class DatabaseContext: DbContext
{
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<FileType> FileTypes { get; set; }
    public DbSet<MediaFile> MediaFiles { get; set; }
    public DbSet<Reaction> Reactions { get; set; }
    public DbSet<ReactionType> ReactionTypes { get; set; }
    
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }
}