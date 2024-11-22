using Common.Attributes;
using LoggerService;
using Microsoft.Extensions.Configuration;
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
        private static readonly object _lock = new object();
        private static IConnection? _connection;
        private static IModel? _channel;
        private static RabbitMQSettings _rabbitMQSettings = new RabbitMQSettings();
        public static readonly string ExchangeName = "log.direct";
        public static readonly string ExchangeType = "direct";
        public static readonly string QueueName = "log";
        public static readonly string RoutingKey = "";

        public static void Configure(IConfiguration configuration)
        {
            configuration.GetSection("RabbitMQ").Bind(_rabbitMQSettings);
        }

        public static ConnectionFactory Factory
        {
            get
            {
                lock (_lock)
                {
                    _factory ??= new ConnectionFactory
                        {
                            HostName = _rabbitMQSettings.Host,
                            UserName = _rabbitMQSettings.User,
                            Password = _rabbitMQSettings.Password,
                            VirtualHost = _rabbitMQSettings.VirtualHost,
                            Port = _rabbitMQSettings.Port
                        };
                    return _factory;
                }
            }
        }

        public static void SendToRabbit<T>(T item, ELogAction action, string logCreatorId, List<string>? recipients = null) where T : class
        {
            try
            {
                PropertyInfo keyProperty = typeof(T).GetProperties()
                                      .FirstOrDefault(prop => prop.GetCustomAttributes(typeof(KeyAttribute), false).Any())!;

                var guidAttribute = (GuidAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(GuidAttribute))!;
                var desciptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(DescriptionAttribute))!;

                CreatingLogModel creatingLogModel = new CreatingLogModel
                {
                    Action = action,
                    Entity = JsonConvert.SerializeObject(item),
                    EntityPk = typeof(T).GetProperties().FirstOrDefault(prop => prop.GetCustomAttributes(typeof(KeyAttribute), false).Any())?.GetValue(item)!.ToString(),
                    EntityType = guidAttribute.EntityGuid,
                    EntityName = desciptionAttribute.Text,
                    UserId = logCreatorId,
                    Time = DateTime.UtcNow.Ticks
                };

                if (recipients != null)
                {
                    creatingLogModel.Recipients.AddRange(recipients);
                }

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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}