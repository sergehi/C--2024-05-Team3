using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using LoggerService;

namespace Common
{
    public static class RabbitMQService
    {
        private static ConnectionFactory? _factory;
        private static IConnection? _connection;
        private static IModel? _channel;
        public static readonly string ExchangeName = "log.direct";
        public static readonly string ExchangeType = "direct";
        public static readonly string QueueName = "log";
        public static readonly string RoutingKey = "";

        public static ConnectionFactory Factory
        {
            get
            {
                _factory ??= new ConnectionFactory()
                {
                    HostName = Environment.GetEnvironmentVariable("RabbitMQ__Host"),
                    UserName = Environment.GetEnvironmentVariable("RabbitMQ__User"),
                    Password = Environment.GetEnvironmentVariable("RabbitMQ__Password"),
                    VirtualHost = Environment.GetEnvironmentVariable("RabbitMQ__VirtualHost")
                };
                return _factory;
            }
        }

        public static void SendToRabbit(CreatingLogModel createLog)
        {
            if (_connection == null)
            {
                _connection = Factory.CreateConnection();
                _channel = _connection.CreateModel();
            }

            _channel.ExchangeDeclare(exchange: ExchangeName, ExchangeType);
            string message = JsonConvert.SerializeObject(createLog);
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: ExchangeName, routingKey: string.Empty, basicProperties: null, body: body);
        }

        public static Task SendToRabbitAsync(CreatingLogModel createLog)
        {
            if (_connection == null)
            {
                _connection = Factory.CreateConnection();
                _channel = _connection.CreateModel();
            }

            _channel.ExchangeDeclare(exchange: ExchangeName, ExchangeType);
            string message = JsonConvert.SerializeObject(createLog);
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: ExchangeName, routingKey: string.Empty, basicProperties: null, body: body);
            return Task.CompletedTask;
        }
    }
}