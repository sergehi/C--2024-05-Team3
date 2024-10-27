﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaslsService.BusinessLogic.DTO
{
    public class NodeDTO
    {
        public long Id { get; set; }
        public long InternalNum { get; set; } // Внутренний номер для сопоставления объектов приходящих по grpc
        public long DefinitionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Terminator { get; set; }
        public string? Description { get; set; }
    }
}