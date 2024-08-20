using ChatService.Entities;
using ChatService.Infrastructure.EntityFramework;
using ChatService.Services.Repositories.Abstractions;
using Common.Repositories;

namespace ChatService.Infrastructure.Repositories.Implementations;

public class ReactionRepository: Repository<Reaction>, IReactionRepository
{
    public ReactionRepository(DatabaseContext context) : base(context)
    {
    }
}