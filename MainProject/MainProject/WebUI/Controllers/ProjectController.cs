using System.Linq.Expressions;
using System.Text;
using BLL.Abstraction.Interfaces;
using Core;
using Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebUI.Models;
using Task = System.Threading.Tasks.Task;
using TaskProject = Core.Task;

namespace WebUI.Controllers;

public class ProjectController : Controller
{
    private readonly IProjectService _projectService;
    private readonly IUserService _userService;
    private readonly ITaskService _taskService;
    private readonly ITaskFileService _taskFileService;
    private readonly IUserProjectService _userProjectService;

    public ProjectController(IProjectService projectService, IUserService userService,
        IUserProjectService userProjectService, ITaskFileService taskFileService, ITaskService taskService)
    {
        _projectService = projectService;
        _userService = userService;
        _taskService = taskService;
        _taskFileService = taskFileService;
        _userProjectService = userProjectService;
    }
    
    public async Task<IActionResult> Index(Guid taskId)
    {
        try
        {
            if (_userService.User == null)
            {
                var modelNull = new ProjectUserViewModel()
                {
                    User = null
                };
            
                return View(modelNull);
            }
        
            if (TempData.TryGetValue("CurrentProject", out var projectData))
            {
                var task = await _taskService.GetById(taskId);
                
                var projectDetailsViewModel = JsonConvert.DeserializeObject<ProjectDetailsViewModel>(projectData.ToString());
                TempData.Remove("CurrentProject");
                
                if (task != null && task.Description != null)
                {
                    await _projectService.SentEmail(projectDetailsViewModel.Project.Id, task.Id, $"Your task - {task.Title} was changed");
                }
            }

            var projects = await _projectService.GetList(p => p.UserProjects.Any(up => up.UserId == _userService.User.Id), 
                null, "Tasks.Files,UserProjects.User");

            await UpdateRate(projects.ToList());
            await _projectService.CheckDeadline(projects.ToList());
            
            var model = new ProjectUserViewModel()
            {
                Projects = projects.OrderBy(p => p.CompletionRate).ToList(),
                User = _userService.User
            };
        
            return View(model);
        }
        catch (Exception e)
        {
            ViewData["IndexError"] = $"Failed to show menu. Error: {e.Message}";
            return View("Error");
        }
    }
    
    public async Task<IActionResult> SelectUsers()
    {
        try
        {
            var users = await _userService.GetListByPredicate(u => u.Id != _userService.User.Id);
            var usersWithValidEmail = users.Where(u => _userService.IsValidEmail(u.Email)).ToList();
            
            return View(usersWithValidEmail);
        }
        catch (Exception e)
        {
            ViewData["SelectError"] = $"Failed to select users. Error: {e.Message}";
            return View("Error");
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> AddUsersToProject(List<Guid> selectedUsers)
    {
        try
        {
            var users = await _userService.GetListByPredicate(u => selectedUsers.Contains(u.Id));

            HttpContext.Session.SetString("SelectedUsers", JsonConvert.SerializeObject(users));
        
            return RedirectToAction("Create", "Task");
        }
        catch (Exception e)
        {
            return View("Error");
        }
    }
    
    public async Task<IActionResult> SelectNewUsers(Guid id)
    {
        try
        {
            var project = await _projectService.GetById(id,"UserProjects.User");
            if (project == null)
            {
                return NotFound();
            }
        
            var allUsers = await _userService.GetListByPredicate(u => u.Id != _userService.User.Id);
            var newUsers = allUsers.Where(u => !project.UserProjects.Any(up => up.UserId == u.Id)).ToList();
            var usersWithValidEmail = newUsers.Where(u => _userService.IsValidEmail(u.Email)).ToList();

            ViewBag.ProjectId = project.Id; 
        
            return View(usersWithValidEmail);
        }
        catch (Exception e)
        {
            return View("Error");
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> AddNewUsersToProject(List<Guid> selectedUsers, Guid projectId)
    {
        try
        {
            var project = await _projectService.GetById(projectId, "UserProjects.User");
            var users = await _userService.GetListByPredicate(u => selectedUsers.Contains(u.Id));
        
            foreach (var user in users)
            {
                var userProject = new UserProject()
                {
                    User = user,
                    UserId = user.Id,
                    Project = project,
                    ProjectId = project.Id
                };
            
                project.UserProjects.Add(userProject);
            }
        
            await _projectService.Update(project.Id, project);
        
            return RedirectToAction("Index", "Project");
        }
        catch (Exception e)
        {
            return View("Error");
        }
    }

    public async Task<IActionResult> SelectNewTask(Guid id)
    {
        try
        {
            var project = await _projectService.GetById(id,"Tasks.Files");
            if (project == null)
            {
                return NotFound();
            }

            ViewBag.ProjectId = project.Id; 
        
            return View();
        }
        catch (Exception e)
        {
            ViewData["SelectError"] = $"Failed to select new task. Error: {e.Message}";
            return View();
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> AddNewTaskToProject(TaskViewModel model, Guid projectId)
    {
        try
        {
            var project = await _projectService.GetById(projectId,"Tasks.Files");
        
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
                var file = new TaskFile
                {
                    FileName = model.File.FileName,
                    FileData = new byte[model.File.Length],
                    CreationDate = DateTime.Now
                };
            
                using (var stream = new MemoryStream())
                {
                    await model.File.CopyToAsync(stream);
                    file.FileData = stream.ToArray();
                }
            
                await _taskFileService.CreateFile(file);
            
                task.Files.Add(file);
                await _taskService.Update(task.Id, task);
            }
        
            project.Tasks.Add(task);
            await _projectService.Update(project.Id, project);
        
            return RedirectToAction("Index", "Project");
        }
        catch (Exception e)
        {
            ViewData["SelectError"] = $"Failed to select users. Error: {e.Message}";
            return View("Error");
        }
    }

    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectViewModel model)
    {
        try
        {
            var selectedUsersJson = HttpContext.Session.GetString("SelectedUsers");
            HttpContext.Session.Remove("SelectedUsers");
            var selectedTasksJson = HttpContext.Session.GetString("SelectedTasks");
            HttpContext.Session.Remove("SelectedTasks");

            if (!string.IsNullOrEmpty(selectedUsersJson) && !string.IsNullOrEmpty(selectedTasksJson))
            {
                var selectedUsers = JsonConvert.DeserializeObject<List<User>>(selectedUsersJson);
                var selectedTasks = JsonConvert.DeserializeObject<List<TaskProject>>(selectedTasksJson);
            
                var project = new Project()
                {
                    Title = model.Title,
                    Description = model.Description,
                    CompletionRate = 0,
                    Tasks = new List<TaskProject>(),
                    UserProjects = new List<UserProject>()
                };

                await _projectService.CreateProject(project);

                foreach (var task in selectedTasks)
                {
                    project.Tasks.Add(task);
                }
            
                foreach (var user in selectedUsers)
                {
                    var userProject = new UserProject()
                    {
                        User = user,
                        UserId = user.Id,
                        Project = project,
                        ProjectId = project.Id
                    };
            
                    project.UserProjects.Add(userProject);
                }
            
                var userProjectNow = new UserProject()
                {
                    User = _userService.User,
                    UserId = _userService.User.Id,
                    Project = project,
                    ProjectId = project.Id
                };
            
                project.UserProjects.Add(userProjectNow);

                await _projectService.Update(project.Id, project);
            
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View();
            }
        }
        catch (Exception e)
        {
            ViewData["CreationError"] = $"Failed to select users. Error: {e.Message}";
            return View();
        }
    }
    
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var project = await _projectService.GetById(id);
            if (project == null)
            {
                return NotFound();
            }
        
            return View(project);
        }
        catch (Exception e)
        {
            ViewData["SelectError"] = $"Failed to select users. Error: {e.Message}";
            return View("Error");
        }
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Project project)
    {
        try
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            await _projectService.UpdateAll(id, project);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            ViewData["IndexError"] = $"Failed to edit project. Error: {e.Message}";
            return View(nameof(Index));
        }
    }
    
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var project = await _projectService.GetById(id);
            if (project == null)
            {
                return NotFound();
            }
        
            return View(project);
        }
        catch (Exception e)
        {
            ViewData["IndexError"] = $"Failed to delete project. Error: {e.Message}";
            return View(nameof(Index));
        }
    }
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            await _projectService.DeleteProject(id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            ViewData["IndexError"] = $"Failed to delete project. Error: {e.Message}";
            return View(nameof(Index));
        }
    }
    
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var project = await _projectService.GetById(id, "Tasks.Files,UserProjects.User");

            if (project == null)
            {
                return NotFound();
            }
        
            var projectDetailsViewModel = new ProjectDetailsViewModel
            {
                Project = project,
                User = _userService.User
            };
        
            var jsonSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            TempData["CurrentProject"] = JsonConvert.SerializeObject(projectDetailsViewModel, jsonSettings);

            return View(projectDetailsViewModel);
        }
        catch (Exception e)
        {
            ViewData["IndexError"] = $"Failed to show details. Error: {e.Message}";
            return View("Error");
        }
    }
    
    public async Task<IActionResult> DeleteUser(Guid projectId, Guid userProjectId)
    {
        try
        {
            var project = await _projectService.GetById(projectId, "UserProjects.User");
            var userProject = await _userProjectService.GetById(userProjectId);

            project.UserProjects.Remove(userProject);
            
            await _projectService.Update(project.Id, project);
            
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            ViewData["IndexError"] = $"Failed to show details. Error: {e.Message}";
            return View("Error");
        }
    }

    private async Task UpdateRate(List<Project> projects)
    {
        foreach (var project in projects)
        {
            await _projectService.UpdateCompletionRate(project.Id);
        }
    }
}