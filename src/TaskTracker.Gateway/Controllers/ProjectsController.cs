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
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "tasksservice company projects")]
    public class ProjectsController : ControllerBase
    {
        private readonly TasksServiceProto.TasksServiceProto.TasksServiceProtoClient _client;
        public ProjectsController(TasksServiceProto.TasksServiceProto.TasksServiceProtoClient client)
        {
            _client = client;
        }

        //rpc GetCompanyProjects(CompanyProjectsRequest) returns(CompanyProjectsReply);
        [HttpGet("{companyId}/{projId?}")]
        public async Task<IActionResult> GetCompanyProjects(long companyId, long projId = 0)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var request = new CompanyProjectsRequest() { CompanyId = companyId, ProjectId = projId };
                var response = await _client.GetCompanyProjectsAsync(request);
                return Ok(response.Projects);
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



        //rpc CreateCompanyProject(CreateProjectRequest) returns(PkMessage);
        [HttpPost("{companyId}/{name}/{description}")]
        public async Task<IActionResult> CreateCompanyProject(long companyId, string name, string description)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var request = new CreateProjectRequest() {  UserId = currentUserId,  Project = new  CompanyProject() { Id = 0, CompanyId = companyId, Name = name, Description = description } };
                var response = await _client.CreateCompanyProjectAsync(request);
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

        //rpc ModifyCompanyProject(ModifyProjectRequest) returns(BoolReply);
        [HttpPut("{id}/{companyId}/{name}/{description}")]
        public async Task<IActionResult> ModifyCompanyProject(long id, long companyId, string name, string description)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var request = new ModifyProjectRequest() { UserId = currentUserId, ChangeFlags = 0, Project = new CompanyProject () { Id = id, CompanyId = companyId, Name = name, Description = description } };
                var response = await _client.ModifyCompanyProjectAsync(request);
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

        //rpc DeleteCompanyProject(DeleteProjectRequest) returns(BoolReply);
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompanyProject(long id)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var template = new DeleteProjectRequest() { ProjectId = id, UserId = currentUserId};
                var response = await _client.DeleteCompanyProjectAsync(template);
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
