using Logger.BusinessLogic.DTO.Log;
using Logger.BusinessLogic.Services.Abstractions;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client.Events;
using AutoMapper;

namespace LoggerService.Services
{
    public class RabbitMqMessageHandler
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<LogService> _logger;

        public RabbitMqMessageHandler(IServiceScopeFactory serviceScopeFactory, IMapper mapper, ILogger<LogService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task HandleMessageAsync(object? sender, BasicDeliverEventArgs e)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    var service = scope.ServiceProvider.GetRequiredService<ILogService>();
                    var creatingLog = JsonSerializer.Deserialize<CreatingLogModel>(Encoding.UTF8.GetString(e.Body.ToArray()));
                    CreateLogDTO dto = _mapper.Map<CreateLogDTO>(creatingLog);
                    _ = await service.CreateAsync(dto);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "CreateLogAsync: {Request}", Encoding.UTF8.GetString(e.Body.ToArray()));
                }
            }
        }
    }
}
