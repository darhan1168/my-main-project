using System.ComponentModel.DataAnnotations.Schema;

namespace Core;

public class TaskFile : BaseEntity
{
    public string FileName { get; set; }
    public byte[] FileData { get; set; }
    public DateTime CreationDate { get; set; }
}