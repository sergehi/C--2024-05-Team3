using System.ComponentModel.DataAnnotations.Schema;
using Common;
using Common.Repositories;

namespace ChatService.Entities;

public class Reaction : IEntity<int>
{
    public int Id { get; set; }
    public int MessageId { get; set; }
    public int TypeId { get; set; }
    public int UserId { get; set; }
    public bool IsDelete { get; set; }
    
    [ForeignKey("MessageId")]
    public virtual Message Message { get; set; }
    
    [ForeignKey("TypeId")]
    public virtual ReactionType ReactionType { get; set; }
}