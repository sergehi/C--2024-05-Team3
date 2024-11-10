using System;

namespace TasksService.BusinessLogic.DTO
{
    public class TaskEdgeDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long NodeFrom { get; set; }
        public long NodeTo { get; set; }
    }
}