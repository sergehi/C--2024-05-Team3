using AutoMapper;
using LoggerService.Services;

var builder = WebApplication.CreateBuilder(args);

//AutoMapper
var mapper_config = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<Logger.BusinessLogic.Services.Implementations.Mapping.LogMappingsProfile>();
});
mapper_config.AssertConfigurationIsValid();

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddSingleton<IMapper>(new Mapper(mapper_config));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<LogService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");

app.Run();