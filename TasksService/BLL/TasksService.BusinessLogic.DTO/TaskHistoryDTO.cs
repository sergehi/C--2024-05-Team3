using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;

namespace TasksService.BusinessLogic.DTO
{
    public class TaskHistoryDTO
    {
        public long Id { get; set; }
        public Guid UserId { get; set; }
        public Timestamp ActionDate { get; set; } = new Timestamp();
        public string? NewValue { get; set; }
        public string? OldValue { get; set; }
        public string? ValueType { get; set; }
        public string ActionName { get; set; } = string.Empty;
        public string ActionString { get; set; } = string.Empty;
        public long TaskId { get; set; }
    }

}
