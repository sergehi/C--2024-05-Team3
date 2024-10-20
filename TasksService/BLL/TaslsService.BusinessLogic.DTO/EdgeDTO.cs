using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaslsService.BusinessLogic.DTO
{
    public class EdgeDTO
    {
        public long Id { get; set; }
        public long InternalNum { get; set; } // Внутренний номер для сопоставления объектов приходящих по grpc
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public long NodeFromNum { get; set; }
        public long NodeToNum { get; set; }
    }
}
