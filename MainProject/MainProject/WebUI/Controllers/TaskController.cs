using System.Security.Claims;
using BLL.Abstraction.Interfaces;
using Core;
using Core.Enums;
using Core.Helpers;
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
    private readonly ITaskFileService _taskFileService;
    private readonly ISession _session;

    public User User => GetUserFromSession();
    
    public TaskController(IUserService userService, ITaskService taskService, IHttpContextAccessor httpContextAccessor, 
        ITaskFileService taskFileService)
    {
        _taskService = taskService;
        _userService = userService;
        _taskFileService = taskFileService;
        _session = httpContextAccessor.HttpContext.Session;
    }

    public async Task<ViewResult> Create()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<ActionResult> Create(TaskViewModel model)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(model.Description) || string.IsNullOrWhiteSpace(model.Title))
            {
                ViewData["FieldError"] = "Please all fields must be completed";
                return View(model);
            }
            
            var task = new TaskProject()
            {
                Description = model.Description,
                Title = model.Title,
                Deadline = model.Deadline,
                Files = new List<TaskFile>(),
                TaskPriority = model.TaskPriority,
                TaskProgress = TaskProgress.InProgress,
                Users = new List<User>()
            };
        
            await _taskService.CreateTask(task);
        
            if (model.File != null && model.File.Length > 0)
            {
                var file = await _taskFileService.CreateFile(model.File);
            
                task.Files.Add(file);
                await _taskService.Update(task.Id, task);
            }

            AddTaskInSession(task);
        
            return RedirectToAction("Create", "Project");
        }
        catch (Exception e)
        {
            ViewData["CreatingError"] = $"Failed to create task - {model.Title}. Error: {e.Message}";
            return View(model);
        }
    }

    public async Task<ActionResult> Edit(Guid id)
    {
        try
        {
            var task = await _taskService.GetById(id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }
        catch (Exception e)
        {
            ViewData["EditingError"] = $"Failed to show editing task by {id}. Error: {e.Message}";
            return View("Error");
        }
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(TaskProject task)
    {
        try
        {
            await _taskService.UpdateAll(task.Id, task);

            return RedirectToAction("Index", "Project",  new { id = task.Id, taskId = task.Id });
        }
        catch (Exception e)
        {
            ViewData["EditingError"] = $"Failed to edit task - {task.Title}. Error: {e.Message}";
            return View(task);
        }
    }
    
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            var task = await _taskService.GetById(id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }
        catch (Exception e)
        {
            return View("Error");
        }
    }
    
    [HttpPost]
    public async Task<ActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            await _taskService.DeleteTask(id);
            
            return RedirectToAction("Index", "Project");
        }
        catch (Exception e)
        {
            return View("Error");
        }
    }

    public async Task<ActionResult> Check(Guid id)
    {
        try
        {
            var task = await _taskService.GetById(id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }
        catch (Exception e)
        {
            ViewData["CheckingError"] = $"Failed to check task. Error: {e.Message}";
            return View("Error");
        }
    }
    
    [HttpPost]
    public async Task<ActionResult> ChangeProgress(Guid id)
    {
        try
        {
            var task = await _taskService.GetById(id);
            var user = _userService.User;
        
            if (task == null || user == null)
            {
                return NotFound();
            }

            if (task.TaskProgress == TaskProgress.InProgress)
            {
                if (user.Role == UserRole.Developer)
                {
                    await _taskService.TransitionNewStep(id);
                }
                else
                {
                    ViewData["CheckingError"] = "Sorry, but just developer can change progress now";
                    return View("Check", task);
                }
            }
            else if (task.TaskProgress == TaskProgress.Tested)
            {
                if (user.Role == UserRole.Tester)
                {
                    await _taskService.TransitionNewStep(id);
                }
                else
                {
                    ViewData["CheckingError"] = "Sorry, but just tester can change progress now";
                    return View("Check", task);
                }
            }
            else 
            {
                if (user.Role == UserRole.Stakeholder)
                {
                    await _taskService.TransitionNewStep(id);
                }
                else
                {
                    ViewData["CheckingError"] = "Sorry, but just stakeholder can change progress now";
                    return View("Check", task);
                }
            }

            return RedirectToAction("Index", "Project");
        }
        catch (Exception e)
        {
            ViewData["CheckingError"] = $"Failed to change progress. Error: {e.Message}";
            return View("Error");
        }
    }
    
    public async Task<IActionResult> DownloadFile(Guid fileId)
    {
        try
        {
            var file = await _taskFileService.GetById(fileId);
            if (file == null)
            {
                return NotFound();
            }
        
            var contentType = "application/octet-stream"; 
            return File(file.FileData, contentType, file.FileName);
        }
        catch (Exception e)
        {
            ViewData["CreatingError"] = $"Failed to download file. Error: {e.Message}";
            return View("Error");
        }
    }

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

    private void AddTaskInSession(TaskProject task)
    {
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
    }
}