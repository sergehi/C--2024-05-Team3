using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Attributes;

namespace TasksService.BusinessLogic.DTO
{
    public class CompanyProjectDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description {get;set;} = string.Empty;
        public long CompanyId { get; set; }
    }
}
