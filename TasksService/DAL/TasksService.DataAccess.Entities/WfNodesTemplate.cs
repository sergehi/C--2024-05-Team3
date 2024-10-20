using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksService.DataAccess.Entities
{
    public partial class WfNodesTemplate
    {
        [Key]
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public long DefinitionId { get; set; }
        public virtual WfDefinitionsTemplate Definition { get; set; } = null!;
        public virtual ICollection<WfEdgesTemplate> WfedgesTemplNodeFromNavigations { get; set; } = new List<WfEdgesTemplate>();
        public virtual ICollection<WfEdgesTemplate> WfedgesTemplNodeToNavigations { get; set; } = new List<WfEdgesTemplate>();
        public bool Terminator { get; set; }

        [NotMapped]
        public long InternalNum { get; set; } // Внутренний номер для сопоставления объектов приходящих по grpc

    }
}
