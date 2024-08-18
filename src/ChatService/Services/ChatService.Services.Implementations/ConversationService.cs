using AutoMapper;
using ChatService.Entities;
using ChatService.Services.Abstractions;
using ChatService.Services.Contracts;
using ChatService.Services.Repositories.Abstractions;

namespace ChatService.Services.Implementations
{
    public class ConversationService: IConversationService
    {
        public ConversationService(IMapper mapper, IConversationRepository conversationRepository)
        {
            _mapper = mapper;
            _conversationRepository = conversationRepository;
        }

        private readonly IMapper _mapper;
        private readonly IConversationRepository _conversationRepository;
        public async Task<List<ConversationDto>> GetByTaskId(int taskId)
        {
            var conversation = _conversationRepository.GetWhere(x=>x.TaskId== taskId).ToList();
            return _mapper.Map<List<ConversationDto>>(conversation);
        }

        public async Task<int> CreateAsync(CreatingConversationDto creatingConversationDto)
        {
            var conversation = _mapper.Map<CreatingConversationDto, Conversation>(creatingConversationDto);
            conversation.CreatedDate = DateTime.UtcNow;
            conversation.CreatedBy = 0;
            conversation.UpdatedDate = DateTime.UtcNow;
            conversation.UpdatedBy = 0;

            var createdCourse = await _conversationRepository.AddAsync(conversation);
            return createdCourse.Id;
        }

        public async Task UpdateAsync(int id, UpdatingConversationDto updatingConversationDto)
        {
            var conversation = await _conversationRepository.GetAsync(id);
            if (conversation == null)
            {
                throw new Exception($"Обсуждение с идентификатором {id} не найден");
            }

            conversation.Title = updatingConversationDto.Title;
            conversation.Description = updatingConversationDto.Description;
            conversation.UpdatedDate = DateTime.UtcNow;
            conversation.UpdatedBy = 0;
            _conversationRepository.Update(conversation);
        }

        public async Task CancelAsync(int id)
        {
            var conversation = await _conversationRepository.GetAsync(id);
            conversation.IsCancel = true;
            _conversationRepository.Update(conversation);
        }
    }
}
