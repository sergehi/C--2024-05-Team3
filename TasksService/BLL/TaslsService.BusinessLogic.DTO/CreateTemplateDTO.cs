using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaslsService.BusinessLogic.DTO
{
    public class CreateTemplateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long? CompanyId { get; set; }
        List<NodeDTO> Nodes { get; set; } = new List<NodeDTO>();
        List<EdgeDTO> Edges { get; set; } = new List<EdgeDTO>();
    }
}
