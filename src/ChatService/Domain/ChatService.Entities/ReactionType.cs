using Common;

namespace ChatService.Entities;

public class ReactionType:BaseEntity
{
    public int Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public int CreatedBy { get; set; }
}