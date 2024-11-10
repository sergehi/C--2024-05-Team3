using System;
using System.Collections.Generic;

namespace TasksService.DataAccess.Entities;

public partial class ProjectArea
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public long ProjectId { get; set; }

    public string? Description { get; set; }

    public virtual CompanyProject Project { get; set; } = null!;

    public virtual ICollection<Entities.Task> Tasks { get; set; } = new List<Entities.Task>();
}
