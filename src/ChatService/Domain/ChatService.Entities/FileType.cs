using Common;
using Common.Repositories;

namespace ChatService.Entities;

public class FileType :IEntity<int>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual ICollection<MediaFile> MediaFiles { get; set; }
}