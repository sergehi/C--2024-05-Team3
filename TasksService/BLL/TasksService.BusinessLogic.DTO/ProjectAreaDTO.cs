using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksService.BusinessLogic.DTO
{
    public class ProjectAreaDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public long ProjectId { get; set; }
        public string Description { get; set; } = string.Empty;

    }
}
