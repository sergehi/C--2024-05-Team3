using Microsoft.AspNetCore.Mvc;
using Grpc.Core;
using TaskTracker.Gateway.Helpers;
using TasksServiceProto;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace TaskTracker.Gateway.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    [ApiExplorerSettings(GroupName = "tasksservice")]
    public class TasksServiceController : ControllerBase
    {
        private readonly TasksServiceProto.TasksServiceProto.TasksServiceProtoClient _client;

        public TasksServiceController(TasksServiceProto.TasksServiceProto.TasksServiceProtoClient client)
        {
            _client = client;
        }
        //rpc GetTasksList(TasksListRequest) returns(TasksListReply);
        //rpc GetTask(TaskRequest) returns(TaskFullReply);
        //rpc CreateTask(CreateTaskRequest) returns(PkMessage);
        //rpc DeleteTask(DeleteTaskRequest) returns(BoolReply);
        //rpc GetTaskHistory(PkMessage) returns(TaskHistoryReply);
        //// Modification
        //rpc ModifyTaskUrgency(ModifyTaskLongFieldRequest) returns(BoolReply);
        //rpc ModifyTaskState(ModifyTaskLongFieldRequest) returns(BoolReply);
        //rpc ModifyTaskName(ModifyTaskTextFieldRequest ) returns(BoolReply);
        //rpc ModifyTaskDescription(ModifyTaskTextFieldRequest) returns(BoolReply);
        //rpc ModifyTaskNodeDeadline(ModifyNodeDeadlineRequest) returns(BoolReply);


        /*
        [HttpPost("company")]
        public async Task<IActionResult> CreateCompany(CreateCompanyRequest createCompanyRequest)
        {
            try
            {
                var creatorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                createCompanyRequest.CreatorId = creatorId;
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
        */
    }
}
