using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Common.Attributes;

namespace TasksService.DataAccess.Entities;
[Guid("4CAC88FE-A8CD-4AD3-B42E-A118721272D0")]
[Description("Задача")]
public partial class Task
{
    [Key]
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public Guid CreatorId { get; set; } = Guid.Empty;

    public DateTime CreationDate { get; set; }

    public DateTime? DeadlineDate { get; set; }

    public long TemplateId { get; set; }

    public long Urgency { get; set; }

    public long CompanyId { get; set; }

    public long? ProjectId { get; set; }

    public long? AreaId { get; set; }

    public long? CurrentNodeId { get; set; }

    [Timestamp]
    public byte[]? RowVersion { get; set; }

    public virtual ProjectArea? Area { get; set; }

    public virtual TasksCompany Company { get; set; } = null!;

    public virtual CompanyProject? Project { get; set; }

   // public virtual TaskNode? CurrentNode { get; set; }

    public virtual WfDefinitionsTemplate Template { get; set; } = null!;

    public virtual Urgency UrgencyNavigation { get; set; } = null!;

    public virtual ICollection<TaskNode> Nodes { get; set; } = new List<TaskNode>();

}
