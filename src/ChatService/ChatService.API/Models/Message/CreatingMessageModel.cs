namespace ChatService.API.Models.Message;

public class CreatingMessageModel
{
    public int ConversationId { get; set; }
    public int ReplyToMessageId { get; set; }
    public int TypeId { get; set; }
    public string Text { get; set; }
}