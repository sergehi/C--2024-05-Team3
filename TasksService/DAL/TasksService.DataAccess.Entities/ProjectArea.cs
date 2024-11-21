using System;
using System.Collections.Generic;
using Common.Attributes;

namespace TasksService.DataAccess.Entities;
[Guid("4C05D83E-B9A1-43C5-8352-537B8FF79117")]
public partial class ProjectArea
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public long ProjectId { get; set; }

    public string? Description { get; set; }

    public virtual CompanyProject Project { get; set; } = null!;

    public virtual ICollection<Entities.Task> Tasks { get; set; } = new List<Entities.Task>();
}
