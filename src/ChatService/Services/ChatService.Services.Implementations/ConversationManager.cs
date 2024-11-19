using AutoMapper;
using ChatService.Entities;
using ChatService.Services.Abstractions;
using ChatService.Services.Contracts;
using ChatService.Services.Repositories.Abstractions;

namespace ChatService.Services.Implementations
{
    public class ConversationManager: IConversationService
    {
        public ConversationManager(IMapper mapper, IConversationRepository conversationRepository)
        {
            _mapper = mapper;
            _conversationRepository = conversationRepository;
        }

        private readonly IMapper _mapper;
        private readonly IConversationRepository _conversationRepository;
        public async Task<List<ConversationDto>> GetAsync(int taskId)
        {
            var conversation = _conversationRepository.GetWhere(x=>x.TaskId== taskId).ToList();
            return _mapper.Map<List<ConversationDto>>(conversation);
        }

        public Task<int> CreateAsync(CreatingConversationDto creatingConversationDto)
        {
                //var conversation = _mapper.Map<CreatingConversationDto, Conversation>(creatingConversationDto);
                var conversation = new Conversation();
                conversation.Description = creatingConversationDto.Description;
                conversation.Title = creatingConversationDto.Title;
                conversation.TaskId = creatingConversationDto.TaskId;
                conversation.CreatedDate = DateTime.UtcNow;
                conversation.CreatedBy = 0;
                conversation.UpdatedDate = DateTime.UtcNow;
                conversation.UpdatedBy = 0;
                conversation.IsCancel = false;

                var createdConversation = _conversationRepository.Add(conversation);
                _conversationRepository.SaveChanges();
                return Task.FromResult<int>(createdConversation.Id);
        }

        public Task UpdateAsync(int id, UpdatingConversationDto updatingConversationDto)
        {
            var conversation = _conversationRepository.GetWhere(x=>x.Id == id).FirstOrDefault();
            if (conversation == null)
            {
                throw new Exception($"Обсуждение с идентификатором {id} не найден");
            }

            conversation.Title = updatingConversationDto.Title;
            conversation.Description = updatingConversationDto.Description;
            conversation.UpdatedDate = DateTime.UtcNow;
            conversation.UpdatedBy = 0;
            _conversationRepository.Update(conversation);
            _conversationRepository.SaveChanges();
            return Task.FromResult(true);
        }

        public Task CancelAsync(int id)
        {
            var conversation = _conversationRepository.GetWhere(x=>x.Id == id).FirstOrDefault();
            conversation.IsCancel = true;
            _conversationRepository.Update(conversation);
            _conversationRepository.SaveChanges();
            return Task.FromResult(true);
        }
    }
}
