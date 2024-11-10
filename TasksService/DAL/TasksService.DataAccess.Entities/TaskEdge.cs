using System;
using System.Collections.Generic;

namespace TasksService.DataAccess.Entities;

public partial class TaskEdge
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public long NodeFrom { get; set; }

    public long NodeTo { get; set; }

    public virtual TaskNode NodeFromNavigation { get; set; } = null!;

    public virtual TaskNode NodeToNavigation { get; set; } = null!;
}
