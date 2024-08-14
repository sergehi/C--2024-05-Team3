using System;
using System.Collections.Generic;

namespace TasksService.Models;

public partial class WfdefinitionsTempl
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public long CompanyId { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<WfnodesTempl> WfnodesTempls { get; set; } = new List<WfnodesTempl>();
}
