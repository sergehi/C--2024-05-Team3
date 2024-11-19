using ChatService.Entities;
using ChatService.Infrastructure.EntityFramework;
using ChatService.Services.Repositories.Abstractions;
using Common.Repositories;

namespace ChatService.Infrastructure.Repositories.Implementations;

public class MessageRepository: Repository<Message, int>, IMessageRepository
{
    public MessageRepository(DatabaseContext context) : base(context)
    {
    }
}