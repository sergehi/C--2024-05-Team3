using Common;

namespace ChatService.Entities;

public class ReactionType:BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public virtual ICollection<Reaction> Reactions { get; set; }
}