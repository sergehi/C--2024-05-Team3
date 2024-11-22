using Grpc.Core;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Gateway.Helpers;
using TasksServiceProto;

namespace TaskTracker.Gateway.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "tasksservice urgencies")]
    public class UrgenciesController : ControllerBase
    {
        private readonly TasksServiceProto.TasksServiceProto.TasksServiceProtoClient _client;
        public UrgenciesController(TasksServiceProto.TasksServiceProto.TasksServiceProtoClient client)
        {
            _client = client;
        }

        // Task urgencies
        //rpc GetUrgencies(TasksUrgenciesListRequest) returns(TasksUrgenciesListReply);
        [HttpGet("{urgId?}")]
        public async Task<IActionResult> GetUrgencies(long urgId = 0)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var request = new TasksUrgenciesListRequest() { Id = urgId };
                var response = await _client.GetUrgenciesAsync(request);
                return Ok(response.Urgenicies);
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

        //rpc CreateUrgency(CreateUrgencyRequest) returns(CreateUrgencyReply);
        [HttpPost("{name}/{description}")]
        public async Task<IActionResult> CreateUrgency(string name, string description)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var request = new CreateUrgencyRequest() { UserId = currentUserId, Name = name, Description = description};
                var response = await _client.CreateUrgencyAsync(request);
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


        //rpc ModifyUrgency(ModifyUrgencyRequest) returns(BoolReply);
        [HttpPut("{id}/{name}/{description}")]
        public async Task<IActionResult> ModifyUrgency(long id, string name, string description)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var request = new ModifyUrgencyRequest() { UserId = currentUserId, Urgency = new UrgencyModel() { Id = id, Name = name, Description = description} };
                var response = await _client.ModifyUrgencyAsync(request);
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

        //rpc DeleteUrgency(DeleteUrgencyRequest) returns(BoolReply);
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUrgency(long id)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var template = new DeleteUrgencyRequest() { UrgencyId = id, UserId = currentUserId };
                var response = await _client.DeleteUrgencyAsync(template);
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
