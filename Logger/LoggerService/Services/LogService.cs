using AutoMapper;
using Grpc.Core;
using Logger.BusinessLogic.DTO.Log;
using Logger.BusinessLogic.Services.Abstractions;

namespace LoggerService.Services
{
    public class LogService : Log.LogBase
    {
        private readonly ILogService _service;
        private readonly ILogger<LogService> _logger;
        private readonly IMapper _mapper;

        public LogService(ILogService servise, ILogger<LogService> logger, IMapper mapper)
        {
            _service = servise;
            _logger = logger;
            _mapper = mapper;
        }


        /// <summary>
        /// Получить выборку логов.
        /// </summary>
        public override async Task<LogList> GetLogsAsync(FilterLogModel request, ServerCallContext context)
        {
            var res = new LogList();
            try
            {
                FilterLogDTO filter = _mapper.Map<FilterLogDTO>(request);
                IEnumerable<LogDTO> logs = await _service.GetLogsAsync(filter);
                res.Items.AddRange(_mapper.Map<IEnumerable<LogItem>>(logs));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetLogsAsync: {Request}", request);
            }
            return res;
        }

        /// <summary>
        /// Получить постраничную выборку логов.
        /// </summary>
        public override async Task<LogList> GetPagedLogsAsync(FilterLogModel request, ServerCallContext context)
        {
            var res = new LogList();
            try
            {
                FilterLogDTO filter = _mapper.Map<FilterLogDTO>(request);
                IEnumerable<LogDTO> logs = await _service.GetPagedLogsAsync(filter);
                res.Items.AddRange(_mapper.Map<IEnumerable<LogItem>>(logs));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPagedLogsAsync: {Request}", request);
            }
            return res;
        }

        /// <summary>
        /// Добавить новый лог.
        /// </summary>
        public override async Task<LogId> CreateLogAsync(CreatingLogModel request, ServerCallContext context)
        {
            LogId res = new LogId();
            try
            {
                CreateLogDTO dto = _mapper.Map<CreateLogDTO>(request);
                res.Id = await _service.CreateAsync(dto);

                await _service.CreateAsync(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateLogAsync: {Request}", request);
            }
            return res;
        }

        /// <summary>
        /// Удалить логи за период.
        /// </summary>
        public override async Task<Empty> DeleteLogAsync(FilterPeriod request, ServerCallContext context)
        {
            Empty res = new Empty();
            try
            {
                await _service.DeleteAsync(new DateTime(request.BeginTime), new DateTime(request.EndTime));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteLogAsync: {Request}", request);
            }
            return res;
        }
    }
}
