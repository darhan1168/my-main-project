using System.ComponentModel.DataAnnotations;
using Core;
using Core.Enums;

namespace WebUI.Models;

public class TaskViewModel
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Deadline { get; set; }
    public TaskPriority TaskPriority { get; set; }
    public IFormFile File { get; set; }
}