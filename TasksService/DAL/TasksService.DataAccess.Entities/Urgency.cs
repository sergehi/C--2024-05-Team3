using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Common.Attributes;

namespace TasksService.DataAccess.Entities;
[Guid("603B2C29-F4C1-4FC5-A769-17C1C6236184")]
[Description("Уровень важности")]
public partial class Urgency
{
    [Key]
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Entities.Task> Tasks { get; set; } = new List<Entities.Task>();
}
