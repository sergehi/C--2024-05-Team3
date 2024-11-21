using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TasksServiceProto;
using TaskTracker.Gateway.Helpers;

namespace TaskTracker.Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(GroupName = "tasksservice templates")]
    public class TemplatesController : ControllerBase
    {
        private readonly TasksServiceProto.TasksServiceProto.TasksServiceProtoClient _client;

        public TemplatesController(TasksServiceProto.TasksServiceProto.TasksServiceProtoClient client)
        {
            _client = client;
        }

        [HttpGet("request")]
        public async Task<IActionResult> GetTemplateList(TemplateListRequest request)
        {
            try
            {
                var response = await _client.GetTemplateListAsync(request);
                return Ok(response.Items);
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

        [HttpPost("template")]
        public async Task<IActionResult> CreateTemplate(CreateTemplateRequest template)
        {
            try
            {
                var response = await _client.CreateTemplateAsync(template);
                return Ok(response.Id);
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
        [HttpPut("template")]
        public async Task<IActionResult> UpdateTemplate(UpdateTemplateRequest template)
        {
            try
            {
                var response = await _client.UpdateTemplateAsync(template);
                return Ok(response.Success);
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

        [HttpDelete("template")]
        public async Task<IActionResult> DeleteTemplate(DeleteTemplateRequest template)
        {
            try
            {
                var response = await _client.DeleteTemplateAsync(template);
                return Ok(response.Success);
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
