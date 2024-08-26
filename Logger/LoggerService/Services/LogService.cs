using Grpc.Core;
using Microsoft.Extensions.Configuration;

namespace LoggerService.Services
{
    public class LogService : Log.LogBase
    {
        private readonly ILogger<LogService> _logger;

        public LogService(ILogger<LogService> logger)
        {
            _logger = logger;
        }


        public override Task<LogId> CreateLog(CreateLogRequest request, ServerCallContext context)
        {
            LogId res = new LogId();
            try
            {
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateLog: {Request}", request);
            }
            return Task.FromResult(res);
        }
    }
}
