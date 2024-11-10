using Microsoft.AspNetCore.Mvc;
using Grpc.Core;
using TaskTracker.Gateway.Helpers;
using TasksTemplatesService;

namespace TaskTracker.Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(GroupName = "tasksservice")]
    public class TasksServiceController : ControllerBase
    {
        private readonly TaskTemplates.TaskTemplatesBase _client;

        public TasksServiceController(TaskTemplates.TaskTemplatesBase client)
        {
            _client = client;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            try
            {
                var response = await _client.LoginAsync(loginRequest);
                return Ok(response);
            }
            catch (RpcException ex)
            {
                var httpStatusCode = GrpcStatusCodeMapper.MapGrpcStatusCodeToHttp(ex.StatusCode);
                return StatusCode(httpStatusCode, new { message = ex.Status.Detail });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An unexpected error occurred: {ex.Message}" });
            }
        }
    }
}
