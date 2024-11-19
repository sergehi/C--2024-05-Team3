using AutoMapper;
using Grpc.Core;
using ChatProto;
using ChatService.Services.Abstractions;
using ChatService.Services.Contracts;


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
        // public override Task<GetConversationResponse> GetConversation(GetConversationRequest request, ServerCallContext context)
        // {
        //     // Returning all mock conversations as if they belong to the same task ID
        //     var getConversationList = _conversationService.GetAsync(request.TaskId);
        //     var conversations = _mapper.Map<List<ConversationModel>>(getConversationList);
        //     var response = new GetConversationResponse();
        //     response.Conversations.AddRange(conversations);
        //     return Task.FromResult(response);
        // }
        
        public async override Task<CreateConversationResponse> CreateConversation(CreateConversationRequest request, ServerCallContext context)
        {
            var res = new CreateConversationResponse();
            CreatingConversationDto creatingConversationDto = new CreatingConversationDto();
            creatingConversationDto.TaskId = request.ConversationModel.TaskId;
            creatingConversationDto.Title = request.ConversationModel.Title;
            creatingConversationDto.Description = request.ConversationModel.Description;
            return _mapper.Map<CreateConversationResponse>(await _conversationService.CreateAsync(creatingConversationDto));
        }
    }
}
