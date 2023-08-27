using Core;
using Task = Core.Task;

namespace WebUI.Models;

public class ProjectViewModel
{
    public string Title { get; set; }
    public string Description { get; set; }
    public List<User> Users { get; set; }
    public List<Task> Tasks { get; set; }
}