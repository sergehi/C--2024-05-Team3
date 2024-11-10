using System;

namespace TasksService.BusinessLogic.DTO
{
    public class TaskNodeDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<long> NodeDoers { get; set; }  = new List<long>();
        public long TaskId { get; set; }
        public bool? Terminating { get; set; }
        public long? IconId { get; set; }

    }
}