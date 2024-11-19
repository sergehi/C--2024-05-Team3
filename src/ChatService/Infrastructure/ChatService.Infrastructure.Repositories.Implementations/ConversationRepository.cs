using ChatService.Entities;
using ChatService.Infrastructure.EntityFramework;
using ChatService.Services.Repositories.Abstractions;
using Common.Repositories;

namespace ChatService.Infrastructure.Repositories.Implementations
{
    public class ConversationRepository: Repository<Conversation, int>, IConversationRepository
    {
        public ConversationRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
