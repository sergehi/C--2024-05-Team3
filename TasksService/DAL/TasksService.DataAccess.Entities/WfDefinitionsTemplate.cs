using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Attributes;

namespace TasksService.DataAccess.Entities
{
    [Guid("0DFD4354-6983-42DE-9CEB-02B4E30AA30B")]
    [Description("Шаблон задачи")]
    public partial class WfDefinitionsTemplate
    {
        [Key]
        public long Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public long CompanyId { get; set; }

        public virtual TasksCompany Company { get; set; } = null!;
        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
        public virtual ICollection<WfNodesTemplate> WfnodesTempls { get; set; } = new List<WfNodesTemplate>();

    }
}
