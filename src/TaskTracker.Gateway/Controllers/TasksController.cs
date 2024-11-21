using Microsoft.AspNetCore.Mvc;
using Grpc.Core;
using TaskTracker.Gateway.Helpers;
using TasksServiceProto;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.Design;
using Google.Protobuf.WellKnownTypes;

namespace TaskTracker.Gateway.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    [ApiExplorerSettings(GroupName = "tasksservice")]
    public class TasksController : ControllerBase
    {
        private readonly TasksServiceProto.TasksServiceProto.TasksServiceProtoClient _client;

        public TasksController(TasksServiceProto.TasksServiceProto.TasksServiceProtoClient client)
        {
            _client = client;
        }

        //rpc GetTasksList(TasksListRequest) returns(TasksListReply);
        [HttpGet("list/{companyId?}/{projectId?}/{areaId?}")]
        public async Task<IActionResult> GetTasksList(long companyId = 0, long projectId = 0, long areaId = 0)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var request = new TasksListRequest() { UserId = currentUserId, CompanyId = companyId, ProjectId = projectId, AreaId = areaId };

                var response = await _client.GetTasksListAsync(request);
                return Ok(response.Tasks);
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

        //rpc GetTask(TaskRequest) returns(TaskFullReply);
        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTask(long taskId)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var request = new TaskRequest() { UserId = currentUserId, TaskId = taskId };

                var response = await _client.GetTaskAsync(request);
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

        //rpc GetTaskHistory(PkMessage) returns(TaskHistoryReply);
        [HttpGet("history/{Id}")]
        public async Task<IActionResult> GetTaskHistory(long taskId)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var request = new PkMessage() {  Id = taskId };

                var response = await _client.GetTaskHistoryAsync(request);
                return Ok(response.History);
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



        //rpc CreateTask(CreateTaskRequest) returns(PkMessage);
        [HttpPost("{model}")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskModel model)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var request = new CreateTaskRequest() { UserId = currentUserId, Task = model};
                var response = await _client.CreateTaskAsync(request);
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
        //rpc DeleteTask(DeleteTaskRequest) returns(BoolReply);
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(long id)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var template = new DeleteTaskRequest() { TaskId = id, UserId = currentUserId };
                var response = await _client.DeleteTaskAsync(template);
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
        //rpc ModifyTaskUrgency(ModifyTaskLongFieldRequest) returns(BoolReply);
        [HttpPut("{taskId}/urgency/{urgId}")]
        public async Task<IActionResult> ModifyTaskUrgency(long taskId, long urgId)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var template = new ModifyTaskLongFieldRequest() { UserId = currentUserId, TaskId = taskId, LongValue = urgId};
                var response = await _client.ModifyTaskUrgencyAsync(template);
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
        //rpc ModifyTaskState(ModifyTaskLongFieldRequest) returns(BoolReply);
        [HttpPut("{taskId}/state/{nodeId?}")]
        public async Task<IActionResult> ModifyTaskState(long taskId, long nodeId = 0)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var template = new ModifyTaskLongFieldRequest() { UserId = currentUserId, TaskId = taskId, LongValue = nodeId };
                var response = await _client.ModifyTaskStateAsync(template);
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

        //rpc ModifyTaskName(ModifyTaskTextFieldRequest ) returns(BoolReply);
        [HttpPut("{taskId}/name/{name}")]
        public async Task<IActionResult> ModifyTaskState(long taskId, string name)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var template = new ModifyTaskTextFieldRequest() { UserId = currentUserId, TaskId = taskId, StrValue = name};
                var response = await _client.ModifyTaskNameAsync(template);
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


        //rpc ModifyTaskDescription(ModifyTaskTextFieldRequest) returns(BoolReply);
        [HttpPut("{taskId}/descr/{description}")]
        public async Task<IActionResult> ModifyTaskDescription(long taskId, string description)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var template = new ModifyTaskTextFieldRequest() { UserId = currentUserId, TaskId = taskId, StrValue = description };
                var response = await _client.ModifyTaskDescriptionAsync(template);
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
        //rpc ModifyTaskNodeDeadline(ModifyNodeDeadlineRequest) returns(BoolReply);
        [HttpPut("{taskId}/deadline/{date}")]
        public async Task<IActionResult> ModifyTaskNodeDeadline(long taskId, DateTime deadline)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var template = new ModifyNodeDeadlineRequest() { UserId = currentUserId, TaskId = taskId, DateValue = Timestamp.FromDateTime(deadline.ToUniversalTime()), NodeId = 0};
                var response = await _client.ModifyTaskNodeDeadlineAsync(template);
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

        // TaskRoutes
        //rpc GetFromNodeTransitions(PkMessage) returns(TransitionListReply);
        [HttpGet("edges_from_node/{nodeId}")]
        public async Task<IActionResult> GetFromNodeTransitions(long nodeId)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var request = new PkMessage() { Id = nodeId };

                var response = await _client.GetFromNodeTransitionsAsync(request);
                return Ok(response.Edges);
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

        //rpc GetToNodeTransitions(PkMessage) returns(TransitionListReply);
        [HttpGet("edges_to_node/{nodeId}")]
        public async Task<IActionResult> GetToNodeTransitions(long nodeId)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var request = new PkMessage() { Id = nodeId };

                var response = await _client.GetToNodeTransitionsAsync(request);
                return Ok(response.Edges);
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

        //rpc GetNode(PkMessage) returns(TaskNode);
        [HttpGet("node/{nodeId}")]
        public async Task<IActionResult> GetNode(long nodeId)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var request = new PkMessage() { Id = nodeId };

                var response = await _client.GetNodeAsync(request);
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

        //rpc AppointNodeDoers(AppointDoersRequest) returns(BoolReply);
        [HttpPut("node/doers/{request}")]
        public async Task<IActionResult> AppointNodeDoers([FromBody] AppointDoersRequest request)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                request.UserId = currentUserId;
                var response = await _client.AppointNodeDoersAsync(request);
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
