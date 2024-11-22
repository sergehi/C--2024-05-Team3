using Common.Attributes;
using System.ComponentModel;

namespace TasksService.DataAccess.Entities;

[Guid("457A2175-BDC8-4634-90E5-DD0FDA47DEBA")]
[System.ComponentModel.Description("Запись сотрудника")]
public partial class CompanyEmployee
{
    public long CompanyId { get; set; }

    public Guid EmployeeId { get; set; }

    public virtual TasksCompany Company { get; set; } = null!;
}
