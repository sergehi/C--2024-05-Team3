using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksService.BusinessLogic.DTO
{
    public class CompanyDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}
