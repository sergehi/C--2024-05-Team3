﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksService.DataAccess.Entities
{
    public partial class TasksCompany
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public virtual ICollection<WfDefinitionsTemplate> WfdefinitionsTempls { get; set; } = new List<WfDefinitionsTemplate>();
    }
}
