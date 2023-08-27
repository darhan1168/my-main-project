using Core;
using Task = Core.Task;

namespace WebUI.Models;

public class TaskMenuViewModel
{
    public User User { get; set; }
    public List<Task> Tasks { get; set; }
}