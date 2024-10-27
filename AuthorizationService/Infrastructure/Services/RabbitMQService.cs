using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Options;
using Logger.BusinessLogic.DTO.Log;
using AuthorizationService.Core.Interfaces;
using AuthorizationService.Core.Entities;
using Newtonsoft.Json;

public class RabbitMQService : IRabbitMQService
{
    private readonly RabbitMQSettings _settings;

    public RabbitMQService(IOptions<RabbitMQSettings> settings)
    {
        _settings = settings.Value;
    }

    public Task<IEnumerable<LogDTO>> GetLogsAsync(FilterLogDTO filterLogDTO)
    {
        return Task.FromResult<IEnumerable<LogDTO>>(Enumerable.Empty<LogDTO>());
    }

    public Task<IEnumerable<LogDTO>> GetPagedLogsAsync(FilterLogDTO filterLogDTO)
    {
        return Task.FromResult<IEnumerable<LogDTO>>(Enumerable.Empty<LogDTO>());
    }

    public Task<long> CreateAsync(CreateLogDTO createLogDTO)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _settings.HostName,
            UserName = _settings.UserName,
            Password = _settings.Password
        };

        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange: _settings.Exchange, type: "direct");
            string message = JsonConvert.SerializeObject(createLogDTO);
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: _settings.Exchange, routingKey: _settings.RoutingKey, basicProperties: null, body: body);
        }

        return Task.FromResult(0L);
    }

    public Task DeleteAsync(DateTime begin, DateTime end)
    {
        return Task.CompletedTask;
    }
}
