using Grpc.Core;
using ChatProto;

namespace ChatService.Services
{
    public class ConversationService : Conversation.ConversationBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly List<ConversationModel> _conversations = new List<ConversationModel>
        {
            new ConversationModel { Id = 1, Message = "Hello, how can I help?", SenderId = 100, Timestamp = "2024-01-01T10:00:00Z" },
            new ConversationModel{ Id = 2, Message = "I'm looking for information on gRPC.", SenderId = 200, Timestamp = "2024-01-01T10:01:00Z" }
        };
        public ConversationService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }
        public override Task<GetConversationResponse> GetConversation(GetConversationRequest request, ServerCallContext context)
        {
            // Returning all mock conversations as if they belong to the same task ID
            var response = new GetConversationResponse();
            response.Conversations.AddRange(_conversations);
            return Task.FromResult(response);
        }

    }
}
