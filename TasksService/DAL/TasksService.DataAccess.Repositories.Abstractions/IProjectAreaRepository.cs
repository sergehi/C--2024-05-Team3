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
        public Task<long> CreateProjectArea(long userId, Entities.ProjectArea area);
        public Task<bool> DeleteProjectArea(long userId, long areaId);
        public Task<bool> ModifyProjectArea(long userId, long changeFlags, Entities.ProjectArea area);
    }
}
