namespace ChatService.Services.Contracts.Message;

public class MessageDto
{
    public int Id { get; set; }
    public int ReplyToMessageId { get; set; }
    public int ConversationId { get; set; }
    public int UserId { get; set; }
    public int TypeId { get; set; }
    public string Text { get; set; }
    public DateTime CreatedDate { get; set; }
    public int CreatedBy { get; set; }
    public bool IsDelete { get; set; }
}