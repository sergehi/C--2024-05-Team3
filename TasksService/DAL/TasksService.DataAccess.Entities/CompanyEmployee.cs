﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Common.Attributes;


namespace TasksService.DataAccess.Entities;

[Guid("457A2175-BDC8-4634-90E5-DD0FDA47DEBA")]
[Description("Запись сотрудника")]
public partial class CompanyEmployee
{
    [Key]
    public long Id { get; set; }

    public long CompanyId { get; set; }

    public Guid EmployeeId { get; set; }

    public virtual TasksCompany Company { get; set; } = null!;
}
