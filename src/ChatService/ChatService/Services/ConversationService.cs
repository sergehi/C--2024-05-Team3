using AutoMapper;
using Grpc.Core;
using ChatProto;
using ChatService.Services.Abstractions;
using ChatService.Services.Contracts;
using Google.Protobuf.WellKnownTypes;


namespace ChatService.Services
{
    public class ConversationService : Conversation.ConversationBase
    {
        private readonly IMapper _mapper;
        private readonly IConversationService _conversationService;
        private readonly ILogger<ConversationService> _logger;

        public ConversationService(ILogger<ConversationService> logger, IMapper mapper, IConversationService conversationService)
        {
            _logger = logger;
            _mapper = mapper;
            _conversationService = conversationService;
        }
        
        public override Task<GetConversationResponse> GetConversation(GetConversationRequest request, ServerCallContext context)
        {
            // Returning all mock conversations as if they belong to the same task ID
            var response = new GetConversationResponse();
            var getConversationList = _conversationService.GetAsync(request.TaskId).Result;
            foreach (var conversation in getConversationList)
            {
                ConversationModel conversationModel = new();
                conversationModel.Id = conversation.Id;
                conversationModel.Title = conversation.Title;
                conversationModel.Description = conversation.Description;
                response.Conversations.Add(conversationModel);
            }
            return Task.FromResult(response);
        }
        
        public override Task<CreateConversationResponse> CreateConversation(CreateConversationRequest request, ServerCallContext context)
        {
                var res = new CreateConversationResponse();
                CreatingConversationDto creatingConversationDto =_mapper.Map<CreatingConversationDto>(request);
                res.Id = _conversationService.CreateAsync(creatingConversationDto).Result;
                return Task.FromResult(res);
        }
        
        public override Task<Empty> UpdateConversation(UpdateConversationRequest request, ServerCallContext context)
        {
            var updatingConversationDto =_mapper.Map<UpdatingConversationDto>(request);
            _conversationService.UpdateAsync(request.Id,updatingConversationDto);
            return Task.FromResult(new Empty());
        }
        
    }
}
