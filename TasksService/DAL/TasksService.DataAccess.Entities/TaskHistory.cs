using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TasksService.DataAccess.Entities;

public partial class TaskHistory
{
    [Key]
    public long Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime ActionDate { get; set; }
    public string? ActionValue { get; set; }
    public string? OldValue { get; set; }
    public long TaskId { get; set; }
    public long ActionId { get; set; }
    public long? NodeId { get; set; }
    public virtual TaskAction Action { get; set; } = null!;
    //public virtual Task Task { get; set; } = null!;
}