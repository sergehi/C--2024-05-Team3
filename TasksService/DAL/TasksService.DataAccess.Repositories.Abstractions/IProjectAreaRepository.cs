using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksService.DataAccess.Repositories.Abstractions
{
    public interface IProjectAreaRepository
    {
        public Task<List<Entities.ProjectArea>> GetProjectAreas(long projectId, long areaId);
        public Task<long> CreateProjectArea(Guid userId, Entities.ProjectArea area);
        public Task<bool> DeleteProjectArea(Guid userId, long areaId);
        public Task<bool> ModifyProjectArea(Guid userId, long changeFlags, Entities.ProjectArea area);
    }
}
