using Microsoft.AspNetCore.Mvc;
using AuthorizationService.Shared.Protos;
using Grpc.Core;
using TaskTracker.Gateway.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace TaskTracker.Gateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "authorizationservice")]
    public class AuthorizationServiceController : ControllerBase
    {
        private readonly AuthProtoService.AuthProtoServiceClient _client;

        public AuthorizationServiceController(AuthProtoService.AuthProtoServiceClient client)
        {
            _client = client;
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            try
            {
                var response = await _client.RegisterAsync(registerRequest);
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
