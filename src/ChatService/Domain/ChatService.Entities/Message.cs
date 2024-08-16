using System.ComponentModel.DataAnnotations.Schema;
using Common;

namespace ChatService.Entities;

public class Message : BaseEntity
{
    public int ConversationId { get; set; }
    public int UserId { get; set; }
    public int TypeId { get; set; }
    public string Text { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime CreatedBy { get; set; }
    public bool IsDelete { get; set; }

    [ForeignKey("ConversationId")]
    public virtual Conversation Conversation { get; set; }
    
    [ForeignKey("ReplyToMessageId")]
    public virtual Message ReplyToMessage { get; set; }
    
    [ForeignKey("TypeId")]
    public virtual MessageType MessageType { get; set; }
    
    public virtual ICollection<MediaFile> MediaFiles { get; set; }
}