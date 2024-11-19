using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Options;
using Logger.BusinessLogic.DTO.Log;
using AuthorizationService.Core.Interfaces;
using AuthorizationService.Core.Entities;
using Newtonsoft.Json;

public static class RabbitMQService
{
    private static RabbitMQSettings? _settings;
    private static IConnection _connection;
    private static IModel _channel;

    public static void Init(RabbitMQSettings settings)
    {
        _settings = settings;
    }

    public static void SendToRabbit(CreateLogDTO createLogDTO)
    {
        if (_connection == null)
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = _settings!.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        _channel.ExchangeDeclare(exchange: _settings!.Exchange, type: "direct");
        string message = JsonConvert.SerializeObject(createLogDTO);
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(exchange: _settings.Exchange, routingKey: _settings.RoutingKey, basicProperties: null, body: body);
    }

    public static Task SendToRabbitAsync(CreateLogDTO createLogDTO)
    {
        if (_connection == null)
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = _settings!.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        _channel.ExchangeDeclare(exchange: _settings!.Exchange, type: "direct");
        string message = JsonConvert.SerializeObject(createLogDTO);
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(exchange: _settings.Exchange, routingKey: _settings.RoutingKey, basicProperties: null, body: body);
        return Task.CompletedTask;
    }
}
