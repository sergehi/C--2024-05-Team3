using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using LoggerService;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Common.Attributes;

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

        public static void SendToRabbit<T>(T item, ELogAction action, string logCreatorId) where T : class
        {
            try
            {
                PropertyInfo keyProperty = typeof(T).GetProperties()
                                      .FirstOrDefault(prop => prop.GetCustomAttributes(typeof(KeyAttribute), false).Any())!;

                CreatingLogModel creatingLogModel = new CreatingLogModel
                {
                    Action = action,
                    Entity = JsonConvert.SerializeObject(item),
                    EntityPk = Attribute.GetCustomAttribute(typeof(T), typeof(KeyAttribute))!.ToString(),
                    EntityType = Attribute.GetCustomAttribute(typeof(T), typeof(GuidAttribute))!.ToString(),
                    UserId = logCreatorId,
                    Time = DateTime.UtcNow.Ticks
                };

            if (_connection == null)
            {
                _connection = Factory.CreateConnection();
                _channel = _connection.CreateModel();
            }

            _channel.ExchangeDeclare(exchange: ExchangeName, ExchangeType);
            string message = JsonConvert.SerializeObject(creatingLogModel);
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: ExchangeName, routingKey: string.Empty, basicProperties: null, body: body);
        }
    }
}