using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksService.DataAccess.Entities
{
    public partial class TasksCompany
    {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public virtual ICollection<CompanyProject> CompanyProjects { get; set; } = new List<CompanyProject>();

        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

        public virtual ICollection<WfDefinitionsTemplate> WfdefinitionsTempls { get; set; } = new List<WfDefinitionsTemplate>();
    }
}
