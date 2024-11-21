using Microsoft.AspNetCore.SignalR.Client;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;

namespace LoggerService.Services
{
    public class RabbitService
    {
        private readonly IConfiguration _configuration;
        private readonly RabbitMqMessageHandler _messageHandler;
        private IModel? _channel;

        public string? GatewayServiceUrl;


        public RabbitService(IConfiguration configuration, RabbitMqMessageHandler messageHandler)
        {
            _configuration = configuration;
            _messageHandler = messageHandler;
        }

        public void StartListening()
        {
            //ConnectionFactory factory = new ConnectionFactory()
            //{
            //    HostName = _configuration["RmqSettings:Host"],
            //    VirtualHost = _configuration["RmqSettings:VHost"],
            //    UserName = _configuration["RmqSettings:Login"],
            //    Password = _configuration["RmqSettings:Password"]
            //};
            //IConnection connection = factory.CreateConnection();
            IConnection connection = Common.RabbitMQService.Factory.CreateConnection();
            _channel = connection.CreateModel();
            //string? exchangeName = _configuration["RmqSettings:ExchangeName"];
            //string? queueName = _configuration["RmqSettings:QueueName"];
            //string? routingKey = _configuration["RmqSettings:RoutingKey"];
            string? exchangeName = Common.RabbitMQService.ExchangeName;
            string? queueName = Common.RabbitMQService.QueueName;
            string? routingKey = Common.RabbitMQService.RoutingKey;
            _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(queueName, true, false, false, null);
            _channel.QueueBind(queueName, exchangeName, routingKey);
            var consumer = new EventingBasicConsumer(_channel);
            _messageHandler.Connection = new HubConnectionBuilder()
                .WithUrl($"https://{GatewayServiceUrl}/notifyhub", options => 
                { 
                    options.Headers.Add("userId", Guid.Empty.ToString()); 
                })
                .WithAutomaticReconnect()
                .AddJsonProtocol()
                .Build();
            _messageHandler.Connection.StartAsync().Wait();

            consumer.Received += async (sender, e) =>
            {
                try
                {
                    await _messageHandler.HandleMessageAsync(sender, e);
                    _channel.BasicAck(e.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }

            };
            _channel.BasicConsume(queueName, false, consumer);
        }
    }
}
