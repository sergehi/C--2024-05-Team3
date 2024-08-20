namespace ChatService.API.Models.Message;

public class ReactionModel
{
    public int Id { get; set; }
    public int MessageId { get; set; }
    public int TypeId { get; set; }
    public int UserId { get; set; }
    public bool IsDelete { get; set; }
}