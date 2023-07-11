using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;

namespace Core;

public class Task : BaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Deadline { get; set; }
    public TaskPriority TaskPriority { get; set; }
    public TaskProgress TaskProgress { get; set; }
    public List<User> Users { get; set; }
    public List<TaskFile> Files { get; set; }
}