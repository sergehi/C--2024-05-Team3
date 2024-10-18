using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksService.DataAccess.Entities
{
    public partial class WfDefinitionsTemplate
    {
        public long Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public long CompanyId { get; set; }

        public virtual TasksCompany Company { get; set; } = null!;

        public virtual ICollection<WfNodesTemplate> WfnodesTempls { get; set; } = new List<WfNodesTemplate>();

    }
}
