using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;

namespace TasksService.BusinessLogic.DTO
{
    public class TaskDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CreatorId { get; set; } = Guid.Empty;
        public Timestamp CreationDate { get; set; } = new Timestamp();
        public Timestamp? DeadlineDate { get; set; }
        public long TemplateId { get; set; }
        public long UrgencyId { get; set; }
        public long CompanyId { get; set; }
        public long ProjectId { get; set; }
        public long AreaId { get; set; }
        public long? CurrentNode { get; set; }
        public byte[]? RowVersion { get; set; }
    }
}
