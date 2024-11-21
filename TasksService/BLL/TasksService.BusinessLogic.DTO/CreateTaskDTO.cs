using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Attributes;
using Google.Protobuf.WellKnownTypes;

namespace TasksService.BusinessLogic.DTO
{
    public class CreateTaskDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Timestamp? DeadlineDate { get; set; }
        public long TemplateId { get; set; }
        public long UrgencyId { get; set; }
        public long CompanyId { get; set; }
        public long ProjectId { get; set; }
        public long AreaId { get; set; }
        public long? CurrentNode { get; set; }
    }
}
