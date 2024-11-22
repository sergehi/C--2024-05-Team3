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
    [ApiExplorerSettings(GroupName = "tasksservice")]
    public class CompanyController : ControllerBase
    {
        private readonly TasksServiceProto.TasksServiceProto.TasksServiceProtoClient _client;

        public CompanyController(TasksServiceProto.TasksServiceProto.TasksServiceProtoClient client)
        {
            _client = client;
        }

        //rpc GetCompanies(CompanyRequest) returns(CompaniesReply);
        [HttpGet("{companyId?}")]
        public async Task<IActionResult> GetCompanies(long companyId = 0)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var request = new CompanyRequest() { Id = companyId };
                var response = await _client.GetCompaniesAsync(request);
                return Ok(response.Companies);
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

        //rpc CreateCompany(CreateCompanyRequest) returns(PkMessage);
        [HttpPost("{name}/{description}")]
        public async Task<IActionResult> CreateCompany(string name, string description)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var request = new CreateCompanyRequest() { CreatorId = currentUserId, Name = name, Description = description };
                var response = await _client.CreateCompanyAsync(request);
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

        //rpc ModifyCompany(ModifyCompanyRequest) returns(BoolReply);
        [HttpPut("{companyId}/{name}/{description}")]
        public async Task<IActionResult> ModifyCompany(long companyId, string name, string description)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var request = new ModifyCompanyRequest() { UserId = currentUserId, ChangeFlags = 0, Id = companyId, Name = name, Description = description };
                var response = await _client.ModifyCompanyAsync(request);
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

        //rpc DeleteCompany(DeleteCompanyRequest) returns(BoolReply);
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(long id)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var template = new DeleteCompanyRequest() { CompanyId = id, UserId = currentUserId };
                var response = await _client.DeleteCompanyAsync(template);
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
