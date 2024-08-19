using ChatService.Services.Contracts.Message;

namespace ChatService.Services.Abstractions;

public interface IMessageService
{
    Task<List<MessageDto>> GetAsync(int conversationId);
    Task<int> CreateAsync(CreatingMessageDto creatingCourseDto);
    Task DeleteAsync(int id);
}