using Common;
using Common.Repositories;

namespace ChatService.Entities;

public class MessageType : IEntity<int>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    
    public virtual ICollection<Message> Messages { get; set; }
}