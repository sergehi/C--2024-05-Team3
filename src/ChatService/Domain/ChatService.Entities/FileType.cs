using Common;

namespace ChatService.Entities;

public class FileType : BaseEntity
{
    public int Name { get; set; }
    public DateTime CreatedDate { get; set; }
    public int CreatedBy { get; set; }

    public virtual ICollection<FileType> FileTypes { get; set; }
}