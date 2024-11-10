using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksService.BusinessLogic.DTO
{
    public class CreateTemplateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long? CompanyId { get; set; }
        public List<TemplateNodeDTO> Nodes { get; set; } = new List<TemplateNodeDTO>();
        public List<TemplateEdgeDTO> Edges { get; set; } = new List<TemplateEdgeDTO>();
    }
}
