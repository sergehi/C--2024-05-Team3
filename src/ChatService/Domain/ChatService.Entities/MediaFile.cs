using System.ComponentModel.DataAnnotations.Schema;
using Common;
using Common.Repositories;

namespace ChatService.Entities;

public class MediaFile : IEntity<int>
{
    public int Id { get; set; }
    public int MessageId { get; set; }
    public int TypeId { get; set; }
    public string FullName { get; set; }
    public string Path { get; set; }
    public int Size { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime CreatedBy { get; set; }
    
    [ForeignKey("MessageId")]
    public Message Message { get; set; }
    
    [ForeignKey("TypeId")]
    public FileType FileType { get; set; }
}