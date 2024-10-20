using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaslsService.BusinessLogic.DTO;

namespace TasksService.BusinessLogic.Services.Abstractions
{
    public interface ITasksService
    {
        Task<List<TemplateItemDTO>> GetTemplateList(long Id, long CompanyId);
        Task<long> CreateTemplate(string Name, string Description, long CompanyId, List<NodeDTO> Nodes, List<EdgeDTO> Edges);
        Task<bool> UpdateTemplate(long Id, string Name, string Description, long CompanyId, List<NodeDTO> Nodes, List<EdgeDTO> Edges);
        Task<bool> DeleteTemplate(long Id);
    }

}
