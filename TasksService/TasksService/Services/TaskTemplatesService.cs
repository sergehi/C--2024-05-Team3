using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TasksTemplatesService;
using System.Diagnostics;
using Common.Rpc;
using TasksService.BusinessLogic.Services.Abstractions;
using AutoMapper;
using TaslsService.BusinessLogic.DTO;

namespace TasksService.Services
{
    public class TaskTemplatesService : TaskTemplates.TaskTemplatesBase
    {
        private ITasksService _service;
        private readonly ILogger<TaskTemplatesService> _logger;
        private readonly IMapper _mapper;


        public TaskTemplatesService(ITasksService service, ILogger<TaskTemplatesService> logger, IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }

        public async override Task<TemplateListReply> GetTemplateList(TemplateListRequest request, ServerCallContext context)
        {
            var res = new TemplateListReply();
            var templateItems = await _service.GetTemplateList(request.Id, request.CompanyId);
            res.Items.AddRange(templateItems.Select(item => _mapper.Map<TemplateListItem>(item)));
            return res;
        }

        public async override Task<CreateTemplateReply> CreateTemplate(CreateTemplateRequest request, ServerCallContext context)
        {
            var res = new CreateTemplateReply();
            
            try
            {
                return _mapper.Map<CreateTemplateReply>(await _service.CreateTemplate(request.Name, request.Description, request.CompanyId, _mapper.Map<List<NodeDTO>>(request.Nodes), _mapper.Map<List<EdgeDTO>>(request.Edges)));

            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), $"Произошла ошибка при добавлении записи сервере\n{ex.Message}", ex.StackTrace?? "");
            }

        }

        public async override Task<BoolReply> UpdateTemplate(UpdateTemplateRequest request, ServerCallContext context)
        {
            try
            {
                return _mapper.Map<BoolReply>(await _service.UpdateTemplate(request.Item.Id, request.Item.Name, request.Item.Description, request.Item.CompanyId, _mapper.Map<List<NodeDTO>>(request.Item.Nodes), _mapper.Map<List<EdgeDTO>>(request.Item.Edges)));
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при добавлении записи сервере", ex.StackTrace);
            }
        }

        public async override Task<BoolReply> DeleteTemplate(DeleteTemplateRequest request, ServerCallContext context)
        {
            try
            {
                return _mapper.Map<BoolReply>(await _service.DeleteTemplate(request.Id));
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), $"Произошла ошибка при удалении записи сервере\n{ex.Message}", ex.StackTrace);
            }
        }



    }
}
