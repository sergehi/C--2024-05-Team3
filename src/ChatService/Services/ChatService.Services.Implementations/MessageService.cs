using AutoMapper;
using ChatService.Entities;
using ChatService.Services.Abstractions;
using ChatService.Services.Contracts.Message;
using ChatService.Services.Repositories.Abstractions;

namespace ChatService.Services.Implementations;

public class MessageService: IMessageService
{
    private readonly IMapper _mapper;
    private readonly IMessageRepository _messageRepository;
    
    public MessageService(IMapper mapper, IMessageRepository messageRepository)
    {
        _mapper = mapper;
        _messageRepository = messageRepository;
    }
    
    public async Task<List<MessageDto>> GetAsync(int conversationId)
    {
        var messages = _messageRepository.GetWhere(x=>x.ConversationId== conversationId).ToList();
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

    public async Task DeleteAsync(int id)
    {
        var conversation = await _messageRepository.GetAsync(id);
        conversation.IsDelete = true;
        _messageRepository.Update(conversation);
    }
}