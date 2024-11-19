namespace ChatService.Services.Contracts.Message;

public class ReactionDto
{
    public int Id { get; set; }
    public int MessageId { get; set; }
    public int TypeId { get; set; }
    public int UserId { get; set; }
    public bool IsDelete { get; set; }
}