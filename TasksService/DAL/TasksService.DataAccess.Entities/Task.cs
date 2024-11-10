using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TasksService.DataAccess.Entities;

public partial class Task
{
    [Key]
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public long CreatorId { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime? DeadlineDate { get; set; }

    public long TemplateId { get; set; }

    public long Urgency { get; set; }

    public long CompanyId { get; set; }

    public long? ProjectId { get; set; }

    public long? AreaId { get; set; }

    public long CurrentNode { get; set; }

    [Timestamp]
    public byte[]? RowVersion { get; set; }

    public virtual ProjectArea? Area { get; set; }

    public virtual TasksCompany Company { get; set; } = null!;

    public virtual CompanyProject? Project { get; set; }

    public virtual TaskNode? TaskNode { get; set; }

    public virtual WfDefinitionsTemplate Template { get; set; } = null!;

    public virtual Urgency UrgencyNavigation { get; set; } = null!;
}
