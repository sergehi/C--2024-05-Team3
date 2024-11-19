using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using LoggerService;

namespace Common
{
    public static class RabbitMQService
    {
        private static IConnection? _connection;
        private static IModel? _channel;

        private static ConnectionFactory Factory()
        {
            return new ConnectionFactory()
            {
                HostName = Environment.GetEnvironmentVariable("RabbitMQ__Host"),
                UserName = Environment.GetEnvironmentVariable("RabbitMQ__User"),
                Password = Environment.GetEnvironmentVariable("RabbitMQ__Password"),
                VirtualHost = Environment.GetEnvironmentVariable("RabbitMQ__VirtualHost")
            };
        }

        public static void SendToRabbit(CreatingLogModel createLog)
        {
            if (_connection == null)
            {
                ConnectionFactory factory = Factory();
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
            }

            _channel.ExchangeDeclare(exchange: "log.direct", type: "direct");
            string message = JsonConvert.SerializeObject(createLog);
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "log.direct", routingKey: string.Empty, basicProperties: null, body: body);
        }

        public static Task SendToRabbitAsync(CreatingLogModel createLog)
        {
            if (_connection == null)
            {
                ConnectionFactory factory = Factory();
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
            }

            _channel.ExchangeDeclare(exchange: "log.direct", type: "direct");
            string message = JsonConvert.SerializeObject(createLog);
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "log.direct", routingKey: string.Empty, basicProperties: null, body: body);
            return Task.CompletedTask;
        }
    }
}