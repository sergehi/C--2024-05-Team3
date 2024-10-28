using ChatProto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestGrpcService1;

namespace TaskTracker.Gateway.Controllers
{
    [ApiController]
    [Route("chatservice/[controller]")]
    [ApiExplorerSettings(GroupName = "chatservice")]
    public class ConversationController : ControllerBase
    {
        private readonly Conversation.ConversationClient _client;
        public ConversationController(Conversation.ConversationClient client)
        {
            _client = client;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int taskId)
        {
            var response = await _client.GetConversationAsync(new GetConversationRequest { TaskId = taskId });
            return Ok(response.Conversations);
        }
    }
}
