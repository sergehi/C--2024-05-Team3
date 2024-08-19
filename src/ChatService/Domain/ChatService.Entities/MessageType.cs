using Common;

namespace ChatService.Entities;

public class MessageType:BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public virtual ICollection<Message> Messages { get; set; }
}