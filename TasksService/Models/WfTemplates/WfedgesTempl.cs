using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TasksService.Models;

public partial class WfedgesTempl
{
    [Key]
    public long Id { get; set; }

    public string? Name { get; set; }

    public long NodeFrom { get; set; }

    public long NodeTo { get; set; }

    public virtual WfnodesTempl NodeFromNavigation { get; set; } = null!;

    public virtual WfnodesTempl NodeToNavigation { get; set; } = null!;

    [NotMapped]
    public long InternalNum { get; set; } // Внутренний номер для сопоставления объектов приходящих по grpc
}
