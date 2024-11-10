using System;
using System.Collections.Generic;

namespace TasksService.DataAccess.Entities;

public partial class TaskAction
{
    public long ActionId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public long? FieldType { get; set; }
}
