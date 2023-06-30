using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;

namespace Core;

[Table("Tasks")]
public class Task : BaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Deadline { get; set; }
    public TaskPriority TaskPriority { get; set; }
    public TaskProgress TaskProgress { get; set; }
    
    [ForeignKey("responsible-user_id")]
    public User ResponsibleUser { get; set; }
    
    [ForeignKey("creator_id")]
    public User Creator { get; set; }
    public List<TaskFile> Files { get; set; }
}