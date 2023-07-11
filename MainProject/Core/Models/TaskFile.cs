using System.ComponentModel.DataAnnotations.Schema;

namespace Core;

public class TaskFile : BaseEntity
{
    public string FileName { get; set; } 
    public string FilePath { get; set; } 
    public DateTime CreationDate { get; set; }
    
    [ForeignKey("creator_id")]
    public User CreatedBy { get; set; } 
}