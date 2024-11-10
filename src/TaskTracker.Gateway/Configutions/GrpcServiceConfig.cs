namespace TaskTracker.Gateway.Configutions
{
    public class GrpcServiceConfig
    {
        public string GreeterServiceUrl { get; set; }
        public string ChatServiceUrl { get; set; }
        public string AuthorizationServiceUrl { get; set; }
        public string TasksServiceUrl { get; set; }
    }
}
