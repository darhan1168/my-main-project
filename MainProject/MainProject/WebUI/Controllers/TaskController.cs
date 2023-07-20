using System.Security.Claims;
using BLL.Abstraction.Interfaces;
using Core;
using Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebUI.Models;
using Task = System.Threading.Tasks.Task;
using TaskProject = Core.Task;

namespace WebUI.Controllers;

public class TaskController : Controller
{
    private readonly IUserService _userService;
    private readonly ITaskService _taskService;
    private readonly ISession _session;

    public User User => GetUserFromSession();
    
    public TaskController(IUserService userService, ITaskService taskService, IHttpContextAccessor httpContextAccessor)
    {
        _taskService = taskService;
        _userService = userService;
        _session = httpContextAccessor.HttpContext.Session;
    }
    
    public async Task<ViewResult> TaskMenu()
    {
        var tasks = await _taskService.GetTasksByUserId(User.Id);

        var viewModel = new TaskMenuViewModel
        {
            User = User,
            Tasks = tasks
        };
        
        return View(viewModel);
    }

    public async Task<ViewResult> Create()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<ActionResult> Create(TaskViewModel model)
    {
        var task = new TaskProject()
        {
            Description = model.Description,
            Title = model.Title,
            Deadline = model.Deadline,
            Files = new List<TaskFile>(),
            TaskPriority = model.TaskPriority,
            TaskProgress = TaskProgress.NotStarted,
            Users = new List<User>()
        };
        
        await _taskService.CreateTask(task);
        
        TempData["SuccessMessage"] = "Task has been created successfully.";
        
        //HttpContext.Session.SetString("SelectedTasks", JsonConvert.SerializeObject(task));
        var tasksInSession = HttpContext.Session.GetString("SelectedTasks");
        List<TaskProject> tasks;
        
        if (string.IsNullOrEmpty(tasksInSession))
        {
            tasks = new List<TaskProject>();
        }
        else
        {
            tasks = JsonConvert.DeserializeObject<List<TaskProject>>(tasksInSession);
        }
        
        tasks.Add(task);
        HttpContext.Session.SetString("SelectedTasks", JsonConvert.SerializeObject(tasks));
        
        return RedirectToAction("Create", "Project");
    }

    public async Task<ActionResult> Edit(Guid id)
    {
        var task = await _taskService.GetById(id);
        if (task == null)
        {
            return NotFound();
        }

        return View(task);
    }
    
    [HttpPost]
    public async Task<ActionResult> Edit(TaskProject task)
    {
        if (ModelState.IsValid)
        {
            await _taskService.UpdateAll(task.Id, task);
            return RedirectToAction("TaskMenu", "Task", new { username = User.Username });
        }

        return View(task);
    }
    
    public async Task<ActionResult> Delete(Guid id)
    {
        var task = await _taskService.GetById(id);
        if (task == null)
        {
            return NotFound();
        }

        return View(task);
    }
    
    [HttpPost]
    public async Task<ActionResult> DeleteConfirmed(Guid id)
    {
        await _taskService.DeleteTask(id);
        return RedirectToAction("TaskMenu", "Task", new { username = User.Username });
    }
    
    // public async Task<ViewResult> FindUser()
    // {
    //     var users = await _userService.GetListByPredicate(u => u.Role == UserRole.Developer);
    //     return View(users);
    // }
    
    private User GetUserFromSession()
    {
        var authenticatedUser = _session.GetString("AuthenticatedUser");

        if (!string.IsNullOrEmpty(authenticatedUser))
        {
            var user = JsonConvert.DeserializeObject<User>(authenticatedUser);
            return user;
        }

        return null; 
    }
}