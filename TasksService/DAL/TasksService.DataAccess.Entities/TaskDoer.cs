using System;
using System.Collections.Generic;

namespace TasksService.DataAccess.Entities;

public partial class TaskDoer
{
    public long NodeId { get; set; }

    public Guid EmpoyeeId { get; set; }

    public virtual TaskNode Node { get; set; } = null!;
}
