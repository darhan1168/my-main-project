using Core;

namespace WebUI.Models;

public class ProjectUserViewModel
{
    public List<Project> Projects { get; set; }
    public User User { get; set; }
}