namespace TaskTracker.Gateway.Configutions
{
    public class GrpcServiceConfig
    {
        public string ChatServiceUrl { get; set; }
        public string AuthorizationServiceUrl { get; set; }
        public string TasksServiceUrl { get; set; }
    }
}
