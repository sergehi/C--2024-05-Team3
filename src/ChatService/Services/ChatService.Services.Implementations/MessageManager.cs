using AutoMapper;
using ChatService.Entities;
using ChatService.Services.Abstractions;
using ChatService.Services.Contracts.Message;
using ChatService.Services.Repositories.Abstractions;

namespace ChatService.Services.Implementations;

public class MessageManager : IMessageService
{
    private readonly IMapper _mapper;
    private readonly IMessageRepository _messageRepository;
    private readonly IReactionRepository _reactionRepository;

    public MessageManager(IMapper mapper, IMessageRepository messageRepository, IReactionRepository reactionRepository)
    {
        _mapper = mapper;
        _messageRepository = messageRepository;
        _reactionRepository = reactionRepository;
    }

    public async Task<List<MessageDto>> GetAsync(int conversationId)
    {
        var messages = _messageRepository.GetWhere(x => x.ConversationId == conversationId).ToList();
        return _mapper.Map<List<MessageDto>>(messages);
    }

    public async Task<int> CreateAsync(CreatingMessageDto dto)
    {
        var conversation = _mapper.Map<CreatingMessageDto, Message>(dto);
        conversation.CreatedDate = DateTime.UtcNow;
        conversation.CreatedBy = 0;
        var createdCourse = await _messageRepository.AddAsync(conversation);
        return createdCourse.Id;
    }

    public int Create(CreatingMessageDto dto)
    {
        var conversation = _mapper.Map<CreatingMessageDto, Message>(dto);
        conversation.CreatedDate = DateTime.UtcNow;
        conversation.CreatedBy = 0;
        var createdCourse = _messageRepository.Add(conversation);
        _messageRepository.SaveChanges();
        return createdCourse.Id;
    }
    
    public async Task DeleteAsync(int id)
    {
        var conversation = await _messageRepository.GetAsync(id);
        conversation.IsDelete = true;
        _messageRepository.Update(conversation);
    }
    public void Delete(int id)
    {
        var conversation = _messageRepository.GetWhere(x=>x.Id == id).FirstOrDefault();
        conversation.IsDelete = true;
        _messageRepository.Update(conversation);
        _messageRepository.SaveChanges();
    }

    public async Task<List<ReactionDto>> GetReactionsAsync(int messageId)
    {
        var reactions = _reactionRepository.GetWhere(x => x.MessageId == messageId).ToList();
        return _mapper.Map<List<ReactionDto>>(reactions);
    }

    public async Task AddReactionAsync(int userId, int messageId, int typeId)
    {
        _reactionRepository.AddAsync(new Reaction()
        {
            UserId = 0,
            MessageId = messageId,
            TypeId = typeId
        });
    }


    public async Task RemoveReactionAsync(int id)
    {
        var reaction = await _reactionRepository.GetAsync(id);
        reaction.IsDelete = true;
        _reactionRepository.Update(reaction);
    }
}