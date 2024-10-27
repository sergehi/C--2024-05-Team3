using Microsoft.AspNetCore.Mvc;
using TestGrpcService1;

namespace TaskTracker.Gateway.Controllers
{
    [ApiController]
    [Route("/service1/Greeter")]
    [ApiExplorerSettings(GroupName = "service1")]
    public class Service1Controller : ControllerBase
    {
        private readonly Greeter.GreeterClient _client;

        public Service1Controller(Greeter.GreeterClient client)
        {
            _client = client;
        }

        [HttpGet("sayhello1")]
        public async Task<IActionResult> SayHello1(string name)
        {

            return Ok("1");
        }

        [HttpGet("sayhello")]
        public async Task<IActionResult> SayHello(string name)
        {
            var response = await _client.SayHelloAsync(new HelloRequest { Name = name });
            return Ok(response.Message);
        }

        [HttpGet("saygoodbye")]
        public async Task<IActionResult> SayGoodbye(string name)
        {
            var response = await _client.SayGoodbyeAsync(new GoodbyeRequest { Name = name });
            return Ok(response.Message);
        }
    }
}