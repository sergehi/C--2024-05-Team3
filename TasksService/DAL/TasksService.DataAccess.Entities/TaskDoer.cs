using System;
using System.Collections.Generic;
using Common.Attributes;

namespace TasksService.DataAccess.Entities;
[Guid("A5214682-A8BB-4962-AE91-F4806CCA3E41")]
[Description("Исполнитель задачи")]
public partial class TaskDoer
{
    public long NodeId { get; set; }

    public Guid EmpoyeeId { get; set; }

    public virtual TaskNode Node { get; set; } = null!;
}
