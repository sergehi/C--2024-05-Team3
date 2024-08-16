using Common;

namespace ChatService.Entities
{
    public class Conversation : BaseEntity
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
    }
}