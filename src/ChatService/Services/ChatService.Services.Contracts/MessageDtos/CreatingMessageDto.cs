namespace ChatService.Services.Contracts.Message;

public class CreatingMessageDto
{
    public int ConversationId { get; set; }
    public int ReplyToMessageId { get; set; }
    public int UserId { get; set; }
    public int TypeId { get; set; }
    public string Text { get; set; }
}