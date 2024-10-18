﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksService.DataAccess.Entities
{
    public partial class WfEdgesTemplate
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public long NodeFrom { get; set; }
        public long NodeTo { get; set; }
        public virtual WfNodesTemplate NodeFromNavigation { get; set; } = null!;
        public virtual WfNodesTemplate NodeToNavigation { get; set; } = null!;
        [NotMapped]
        public long InternalNum { get; set; } // Внутренний номер для сопоставления объектов приходящих по grpc
    }
}
