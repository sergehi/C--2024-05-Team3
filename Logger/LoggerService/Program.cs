using Logger.BusinessLogic.Services.Abstractions;
using LoggerService;
using LoggerService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddServices(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<LogService>();

var rabbit = app.Services.GetService<RabbitService>();
if (rabbit != null)
{
    rabbit.GatewayServiceUrl = Environment.GetEnvironmentVariable("ConnectionStrings__GatewayServiceUrl");
    rabbit.StartListening();
}

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");
app.Run();