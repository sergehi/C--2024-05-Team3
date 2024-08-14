using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TasksService.Models;

public partial class WfnodesTempl
{
    [Key]
    public long Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public long DefinitionId { get; set; }

    public virtual WfdefinitionsTempl Definition { get; set; } = null!;

    public virtual ICollection<WfedgesTempl> WfedgesTemplNodeFromNavigations { get; set; } = new List<WfedgesTempl>();

    public virtual ICollection<WfedgesTempl> WfedgesTemplNodeToNavigations { get; set; } = new List<WfedgesTempl>();

    [NotMapped]
    public long InternalNum { get; set; } // Внутренний номер для сопоставления объектов приходящих по grpc

}
