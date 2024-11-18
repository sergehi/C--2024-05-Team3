using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksService.DataAccess.Entities;

namespace TasksService.DataAccess.Repositories.Abstractions
{
    public interface IUrgenciesRepository
    {
        Task<List<Urgency>> GetUrgencies(long id);
        Task<bool> ModifyUrgency(Guid userId, long changeFlags, Urgency urgency);
        Task<long> CreateUrgency(Guid userId, string name, string description);
        Task<bool> DeleteUrgency(Guid userId, long urgencyId);
    }
}
