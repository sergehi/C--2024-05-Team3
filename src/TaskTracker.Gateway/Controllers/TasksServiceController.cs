using Microsoft.AspNetCore.Mvc;
using Grpc.Core;
using TaskTracker.Gateway.Helpers;
using TasksServiceProto;

namespace TaskTracker.Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(GroupName = "tasksservice")]
    public class TasksServiceController : ControllerBase
    {
        private readonly TasksServiceReflection _client;

        public TasksServiceController(TasksServiceReflection client)
        {
            _client = client;
        }

        [HttpPost("company")]
        public async Task<IActionResult> CreateCompany(CreateCompanyRequest createCompanyRequest)
        {
            try
            {
                var response = await _client.CreateCompanyAsync(createCompanyRequest);
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
