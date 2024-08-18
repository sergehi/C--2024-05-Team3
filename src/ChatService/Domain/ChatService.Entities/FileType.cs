using Common;

namespace ChatService.Entities;

public class FileType : BaseEntity
{
    public int Name { get; set; }
    public virtual ICollection<MediaFile> MediaFiles { get; set; }
}