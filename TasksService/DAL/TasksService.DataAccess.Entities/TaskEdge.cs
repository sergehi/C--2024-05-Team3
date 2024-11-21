using System;
using System.Collections.Generic;
using Common.Attributes;

namespace TasksService.DataAccess.Entities;
[Guid("434B765A-935D-4166-B4CE-E909776B202B")]
public partial class TaskEdge
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public long NodeFrom { get; set; }

    public long NodeTo { get; set; }

    public virtual TaskNode NodeFromNavigation { get; set; } = null!;

    public virtual TaskNode NodeToNavigation { get; set; } = null!;
}
