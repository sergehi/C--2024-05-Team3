using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Attributes;

namespace TasksService.DataAccess.Entities
{
    [Guid("4654B1E9-F62F-4886-ADCD-5230904541AC")]
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
        public bool Terminating { get; set; }
        public long? IconId { get; set; }
        [NotMapped]
        public long InternalNum { get; set; } // Внутренний номер для сопоставления объектов приходящих по grpc

    }
}
