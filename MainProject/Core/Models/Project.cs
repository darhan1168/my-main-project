using System.ComponentModel.DataAnnotations.Schema;

namespace Core;

public class Project : BaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public double CompletionRate { get; set; }
    public List<UserProject> UserProjects { get; set; }
    public List<Task> Tasks { get; set; }
}