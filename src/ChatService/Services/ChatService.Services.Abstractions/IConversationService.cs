using ChatService.Services.Contracts;

namespace ChatService.Services.Abstractions;

public interface IConversationService
{
    Task<List<ConversationDto>> GetAsync(int taskId);
    Task<int> CreateAsync(CreatingConversationDto creatingCourseDto);
    Task UpdateAsync(int id, UpdatingConversationDto updatingCourseDto);
    Task CancelAsync(int id);
}