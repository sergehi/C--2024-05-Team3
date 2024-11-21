using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksService.DataAccess.Entities;

namespace TasksService.DataAccess.Repositories.Abstractions
{
    public interface IHistoryRepository
    {
        Task<List<TaskHistory>> GetTaskHistory(long TaskId);

        Task<bool> RegisterTaskUrgencyChanged(Guid UserId, long TaskId, string OldValue, string NewValue);
        Task<bool> RegisterTaskTypeChanged(Guid UserId, long TaskId, string OldValue, string NewValue);
        Task<bool> RegisterTaskStateChanged(Guid UserId, long TaskId, string OldValue, string NewValue);
        Task<bool> RegisterTaskNameChanged(Guid UserId, long TaskId, string OldValue, string NewValue);
        Task<bool> RegisterTaskDescriptionChanged(Guid UserId, long TaskId, string OldValue, string NewValue);
        Task<bool> RegisterTaskDeadlineChanged(Guid UserId, long TaskId, long NodeId, string OldValue, string NewValue);
        Task<bool> RegisterTaskDoersAppointed(Guid UserId, long TaskId, long NodeId, string OldValue, string NewValue);
        Task<bool> RegisterDeleteTask(Guid UserId, long TaskId, string OldValue, string NewValue);
        Task<bool> RegisterCreateTask(Guid UserId, long TaskId, string NewValue);

        Task<List<string>> GetCompanyEmployees(Guid UserId, long CompanyId);
        Task<List<string>> GetProjectEmployees(Guid UserId, long ProjectId);

    }
}
