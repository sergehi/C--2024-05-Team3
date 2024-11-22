namespace Common
{
    public class RabbitMQSettings
    {
        public string Host { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string VirtualHost { get; set; } = string.Empty;
        public int Port { get; set; }
    }
}