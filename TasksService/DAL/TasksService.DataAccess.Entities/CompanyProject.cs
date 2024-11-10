﻿using System;
using System.Collections.Generic;

namespace TasksService.DataAccess.Entities;

public partial class CompanyProject
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public long CompanyId { get; set; }

    public virtual TasksCompany Company { get; set; } = null!;

    public virtual ICollection<ProjectArea> ProjectAreas { get; set; } = new List<ProjectArea>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
