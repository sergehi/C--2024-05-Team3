using System.ComponentModel.DataAnnotations.Schema;
using Common;

namespace ChatService.Entities;

public class MediaFile : BaseEntity
{
    public int MessageId { get; set; }
    public int TypeId { get; set; }
    public string FullName { get; set; }
    public string Path { get; set; }
    public int Size { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime CreatedBy { get; set; }
    public bool IsDelete { get; set; }
    
    public Message Message { get; set; }
    public FileType FileType { get; set; }
}