using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common;
using Common.Repositories;

namespace ChatService.Entities
{
    public class Conversation : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsCancel { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
    }
}