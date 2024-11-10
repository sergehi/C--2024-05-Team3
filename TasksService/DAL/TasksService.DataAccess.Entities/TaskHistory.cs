using System;
using System.Collections.Generic;

namespace TasksService.DataAccess.Entities;

public partial class TaskHistory
{
    public long TaskId { get; set; }

    public long ActionId { get; set; }

    public DateTime ActionDate { get; set; }

    public long UserId { get; set; }

    public string? ActionValue { get; set; }

    public string? OldValue { get; set; }

    public long? NodeId { get; set; }

    public virtual TaskAction Action { get; set; } = null!;

    public virtual Task Task { get; set; } = null!;
}