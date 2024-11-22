using System;
using System.Collections.Generic;
using Common.Attributes;

namespace TasksService.DataAccess.Entities;
[Guid("93FE96E7-68DB-438F-951A-4F6514C33858")]
[Description("Проект")]
public partial class CompanyProject
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public long CompanyId { get; set; }

    public virtual TasksCompany Company { get; set; } = null!;

    public virtual ICollection<ProjectArea> ProjectAreas { get; set; } = new List<ProjectArea>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
