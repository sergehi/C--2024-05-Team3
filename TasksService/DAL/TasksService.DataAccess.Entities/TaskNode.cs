using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TasksService.DataAccess.Entities;

public partial class TaskNode
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public long? OwnerTaskId { get; set; }

    public bool? Terminating { get; set; }

    public long? IconId { get; set; }

    public virtual Task OwnerTask { get; set; } = null!;
    public virtual ICollection<TaskDoer> TaskDoers { get; set; } = new List<TaskDoer>();

    public virtual ICollection<TaskEdge> TaskEdgeNodeFromNavigations { get; set; } = new List<TaskEdge>();

    public virtual ICollection<TaskEdge> TaskEdgeNodeToNavigations { get; set; } = new List<TaskEdge>();
    
    [NotMapped]
    public long TemplateId { get; set; }

}
