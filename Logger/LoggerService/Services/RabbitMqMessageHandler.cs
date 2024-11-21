using Logger.BusinessLogic.DTO.Log;
using Logger.BusinessLogic.Services.Abstractions;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client.Events;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Common.SignalR;

namespace LoggerService.Services
{
    public class RabbitMqMessageHandler
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<LogService> _logger;

        public HubConnection? Connection;

        public RabbitMqMessageHandler(IServiceScopeFactory serviceScopeFactory, IMapper mapper, ILogger<LogService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task HandleMessageAsync(object? sender, BasicDeliverEventArgs e)
        {
            string ActionToString(ELogAction action)
            {
                switch (action)
                {
                    case ELogAction.LaCreate: return " создана";
                    case ELogAction.LaUpdate: return " обновлена";
                    case ELogAction.LaDelete: return " удалена";
                }
                return string.Empty;
            }
            string TitleToString(ELogAction action)
            {
                switch (action)
                {
                    case ELogAction.LaCreate: return "Создание";
                    case ELogAction.LaUpdate: return "Обновление";
                    case ELogAction.LaDelete: return "Удаление";
                }
                return string.Empty;
            }

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    var service = scope.ServiceProvider.GetRequiredService<ILogService>();
                    var creatingLog = JsonSerializer.Deserialize<CreatingLogModel>(Encoding.UTF8.GetString(e.Body.ToArray()));
                    CreateLogDTO dto = _mapper.Map<CreateLogDTO>(creatingLog);
                    _ = await service.CreateAsync(dto);
                    List<string> users = creatingLog.Recipients.ToList();
                    if (!users.Any())
                        return;
                    var message = new SignalMessage()
                    {
                        SenderEntity = Guid.Parse(creatingLog.Entity),
                        Title = TitleToString(creatingLog.Action),
                        Body = creatingLog.EntityName + ActionToString(creatingLog.Action),
                        Recipients = users.Select(user => Guid.Parse(user)).ToList()
                    };
                    if (users.Count == 1 && Guid.Parse(users[0]) == Guid.Empty)
                        Connection?.InvokeAsync("SendBroadcastMessage", message);
                    else
                        Connection?.InvokeAsync("SendMessage", message);

                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "CreateLogAsync: {Request}", Encoding.UTF8.GetString(e.Body.ToArray()));
                }
            }
        }
    }
}
