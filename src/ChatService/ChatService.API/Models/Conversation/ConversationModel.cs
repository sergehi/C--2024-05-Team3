namespace ChatService.API.Models.Conversation;

public class ConversationModel
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string CreatedDate { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedDate { get; set; }
    public string UpdatedBy { get; set; }
    public bool isCancel { get; set; }
}