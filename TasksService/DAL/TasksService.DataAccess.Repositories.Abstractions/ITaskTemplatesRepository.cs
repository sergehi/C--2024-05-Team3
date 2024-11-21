using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksService.DataAccess.Entities;

namespace TasksService.DataAccess.Repositories.Abstractions
{
    public interface ITaskTemplatesRepository
    {
        Task<List<WfDefinitionsTemplate>> GetTemplateList(long Id, long CompanyId);
        Task<long> CreateTemplate(long userId, string Name, string Description, long CompanyId, List<WfNodesTemplate> Nodes, List<WfEdgesTemplate> Edges);
        Task<bool> UpdateTemplate(long userId, long Id, string Name, string Description, long CompanyId, List<WfNodesTemplate> Nodes, List<WfEdgesTemplate> Edges);
        Task<bool> DeleteTemplate(long userId, long Id);
    }
}
