using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.SignalR
{
    public class UserConnection
    {
        public string ConnectionId { get; set; } = string.Empty;
        public Guid UserId { get; set; } = Guid.Empty;  
    }
}
