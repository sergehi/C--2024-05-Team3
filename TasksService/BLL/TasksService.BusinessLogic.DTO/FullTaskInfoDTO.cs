using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksService.BusinessLogic.DTO
{
    public class FullTaskInfoDTO
    {
        public TaskDTO Task { get; set; } = null!;
        public List<TaskNodeDTO> Nodes { get; set; } = null!;
        public List<TaskEdgeDTO> Edges { get; set; } = null!;

    }


}
