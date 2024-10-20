using AutoMapper;
using TasksService.BusinessLogic.Services.Abstractions;
using TasksService.DataAccess.Entities;
using TasksService.DataAccess.Repositories.Abstractions;
using TaslsService.BusinessLogic.DTO;

namespace TasksService.BusinessLogic.Services.Implementations
{
    public class TasksService : ITasksService
    {
        private readonly IMapper _mapper;
        private readonly ITasksRepository _repository;

        public TasksService(ITasksRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<long> CreateTemplate(string Name, string Description, long CompanyId, List<NodeDTO> Nodes, List<EdgeDTO> Edges)
        {
            var fwNodes = _mapper.Map<List<NodeDTO>, List<WfNodesTemplate>>(Nodes);
            var fwEdges = _mapper.Map<List<EdgeDTO>, List<WfEdgesTemplate>>(Edges);
            return await _repository.CreateTemplate(Name, Description, CompanyId, fwNodes, fwEdges);
        }

        public async Task<bool> DeleteTemplate(long Id)
        {
            return await _repository.DeleteTemplate(Id);
        }

        public async Task<List<TemplateItemDTO>> GetTemplateList(long Id, long CompanyId)
        {
            try
            {
                List<TemplateItemDTO> res = _mapper.Map<List<TemplateItemDTO>>(await _repository.GetTemplateList(Id, CompanyId));
                return res;

            }
            catch (Exception ex)
            {
                throw;
            }

            //return _mapper.Map<List<TemplateItemDTO>>(await _repository.GetTemplateList(Id, CompanyId));
        }

        public async Task<bool> UpdateTemplate(long Id, string Name, string Description, long CompanyId, List<NodeDTO> Nodes, List<EdgeDTO> Edges)
        {
            var fwNodes = _mapper.Map<List<NodeDTO>, List<WfNodesTemplate>>(Nodes);
            var fwEdges = _mapper.Map<List<EdgeDTO>, List<WfEdgesTemplate>>(Edges);
            return await _repository.UpdateTemplate(Id, Name, Description, CompanyId, fwNodes, fwEdges);      
        }
    }
}
