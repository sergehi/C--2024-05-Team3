using ChatProto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TaskTracker.Gateway.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/chatservice/[controller]")]
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
