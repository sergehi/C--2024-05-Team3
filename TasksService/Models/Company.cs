using System;
using System.Collections.Generic;

namespace TasksService.Models;

public partial class Company
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<WfdefinitionsTempl> WfdefinitionsTempls { get; set; } = new List<WfdefinitionsTempl>();
}
