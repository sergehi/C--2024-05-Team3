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

        Task<bool> RegisterTaskUrgencyChanged(long UserId, long TaskId, string OldValue, string NewValue);
        Task<bool> RegisterTaskTypeChanged(long UserId, long TaskId, string OldValue, string NewValue);
        Task<bool> RegisterTaskStateChanged(long UserId, long TaskId, string OldValue, string NewValue);
        Task<bool> RegisterTaskNameChanged(long UserId, long TaskId, string OldValue, string NewValue);
        Task<bool> RegisterTaskDescriptionChanged(long UserId, long TaskId, string OldValue, string NewValue);
        Task<bool> RegisterTaskDeadlineChanged(long UserId, long TaskId, long NodeId, string OldValue, string NewValue);
        Task<bool> RegisterTaskDoersAppointed(long UserId, long TaskId, long NodeId, string OldValue, string NewValue);
        Task<bool> RegisterDeleteTask(long UserId, long TaskId, string OldValue, string NewValue);
        Task<bool> RegisterCreateTask(long UserId, long TaskId, string NewValue);

    }
}
