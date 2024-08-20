﻿using ChatService.Services.Contracts.Message;

namespace ChatService.Services.Abstractions;

public interface IMessageService
{
    Task<List<MessageDto>> GetAsync(int conversationId);
    Task<int> CreateAsync(CreatingMessageDto creatingCourseDto);
    Task DeleteAsync(int id);
    Task<List<ReactionDto>> GetReactionsAsync(int messageId);
    Task AddReactionAsync(int userId, int messageId, int typeId);
    Task RemoveReactionAsync(int id);
}