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
    [Route("[controller]")]
    [ApiExplorerSettings(GroupName = "tasksservice company employees")]
    public class EmployeesController : ControllerBase
    {
        private readonly TasksServiceProto.TasksServiceProto.TasksServiceProtoClient _client;

        public EmployeesController(TasksServiceProto.TasksServiceProto.TasksServiceProtoClient client)
        {
            _client = client;
        }



        //rpc GetEmployees(EmployeesRequest) returns(EmployeesReply);
        [HttpGet("{companyId}")]
        public async Task<IActionResult> GetEmployees(long companyId)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var request = new EmployeesRequest() {  CompanyId = companyId };
                var response = await _client.GetEmployeesAsync(request);
                return Ok(response.EmployeeIds);
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

        //rpc AddEmployee(AddEmployeeRequest) returns(BoolReply);
        [HttpPost("{companyId}/{newUersId}")]
        public async Task<IActionResult> CreateCompanyProject(long companyId, string newUserId)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var request = new AddEmployeeRequest() { CreatorId = currentUserId, CompanyId = companyId , NewUserId = newUserId };
                var response = await _client.AddEmployeeAsync(request);
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

        //rpc RemoveEmployee(RemoveEmployeeRequest) returns(BoolReply);
        [HttpDelete("{companyId}/{id}")]
        public async Task<IActionResult> RemoveEmployee(long companyId, long userId )
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var template = new RemoveEmployeeRequest() { DeleterId = currentUserId, CompanyId = companyId };
                var response = await _client.RemoveEmployeeAsync(template);
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
