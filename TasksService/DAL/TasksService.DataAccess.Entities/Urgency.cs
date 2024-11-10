using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TasksService.DataAccess.Entities;

public partial class Urgency
{
    [Key]
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Entities.Task> Tasks { get; set; } = new List<Entities.Task>();
}
