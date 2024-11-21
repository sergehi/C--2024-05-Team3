using Common.Attributes;
using LoggerService;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

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

        public static void SendToRabbit<T>(T item, ELogAction action, string logCreatorId, List<string> recipients) where T : class
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
                    EntityName = Attribute.GetCustomAttribute(typeof(T), typeof(DescriptionAttibute))!.ToString(),
                    UserId = logCreatorId,
                    Time = DateTime.UtcNow.Ticks
                };
                creatingLogModel.Recipients.AddRange(recipients);

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
            catch
            { 
            }
        }
    }
}