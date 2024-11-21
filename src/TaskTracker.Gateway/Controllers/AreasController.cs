using Grpc.Core;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TasksServiceProto;
using TaskTracker.Gateway.Helpers;

namespace TaskTracker.Gateway.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    [ApiExplorerSettings(GroupName = "tasksservice project areas")]
    public class AreasController : ControllerBase
    {
        private readonly TasksServiceProto.TasksServiceProto.TasksServiceProtoClient _client;

        public AreasController(TasksServiceProto.TasksServiceProto.TasksServiceProtoClient client)
        {
            _client = client;
        }



        //rpc GetProjectAreas(ProjectAreasRequest)  returns(ProjectAreasReply);
        [HttpGet("{projId}/{areaId?}")]
        public async Task<IActionResult> GetProjectAreas(long projId, long Id = 0)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var request = new ProjectAreasRequest() { ProjectId = projId,  AreaId = Id };
                var response = await _client.GetProjectAreasAsync(request);
                return Ok(response.Areas);
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

        //rpc CreateProjectArea(CreateProjectAreaRequest)  returns(PkMessage);
        [HttpPost("{projId}/{name}/{description}")]
        public async Task<IActionResult> CreateProjectArea(long projId, string name, string description)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var request = new CreateProjectAreaRequest() { UserId = currentUserId, Area = new ProjectArea() { Id = 0, ProjectId = projId, Name = name, Description = description}};
                var response = await _client.CreateProjectAreaAsync(request);
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


        //rpc ModifyProjectArea(ModifyProjectAreaRequest)  returns(BoolReply);
        [HttpPut("{id}/{projId}/{name}/{description}")]
        public async Task<IActionResult> ModifyProjectArea(long id, long projId, string name, string description)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var request = new ModifyProjectAreaRequest() { UserId = currentUserId, ChangeFlags = 0, Area = new ProjectArea() { Id = id, ProjectId = projId, Name = name, Description = description } };
                var response = await _client.ModifyProjectAreaAsync(request);
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

        //rpc DeleteProjectArea(DeleteProjectAreaRequest)  returns(BoolReply);
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProjectArea(long id)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var template = new DeleteProjectAreaRequest() { AreaId = id, UserId = currentUserId };
                var response = await _client.DeleteProjectAreaAsync(template);
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
