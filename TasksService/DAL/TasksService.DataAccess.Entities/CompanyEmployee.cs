using System;
using System.Collections.Generic;
using Common.Attributes;

namespace TasksService.DataAccess.Entities;

[Guid("457A2175-BDC8-4634-90E5-DD0FDA47DEBA")]
public partial class CompanyEmployee
{
    public long CompanyId { get; set; }

    public Guid EmployeeId { get; set; }

    public virtual TasksCompany Company { get; set; } = null!;
}
