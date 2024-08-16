using System.ComponentModel.DataAnnotations.Schema;
using Common;

namespace ChatService.Entities;

public class Reaction:BaseEntity
{
    public int MessageId { get; set; }
    public int TypeId { get; set; }
    public int UserId { get; set; }
    
    [ForeignKey("MessageId")]
    public virtual Message Message { get; set; }
    
    [ForeignKey("TypeId")]
    public virtual ReactionType ReactionType { get; set; }
}