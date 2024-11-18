using System;
using System.Collections.Generic;

namespace TasksService.DataAccess.Entities;

public partial class CompanyEmployee
{
    public long CompanyId { get; set; }

    public Guid EmployeeId { get; set; }

    public virtual TasksCompany Company { get; set; } = null!;
}
