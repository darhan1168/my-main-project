using BLL.Abstraction.Interfaces;
using Core;
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
    private readonly IUserProjectService _userProjectService;

    public ProjectController(IProjectService projectService, IUserService userService,
        IUserProjectService userProjectService)
    {
        _projectService = projectService;
        _userService = userService;
        _userProjectService = userProjectService;
    }
    
    public async Task<IActionResult> Index()
    {
        if (_userService.User == null)
        {
            var modelNull = new ProjectUserViewModel()
            {
                User = null
            };
            
            return View(modelNull);
        }
        
        var projects = await _projectService.GetListByPredicate(p => p.UserProjects.
            Any(up => up.UserId == _userService.User.Id));

        var model = new ProjectUserViewModel()
        {
            Projects = projects,
            User = _userService.User
        };
        
        return View(model);
    }
    
    public async Task<IActionResult> SelectUsers()
    {
        var users = await _userService.GetListByPredicate(u => u.Username != null);

        return View(users);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddUsersToProject(List<Guid> selectedUsers)
    {
        var users = await _userService.GetListByPredicate(u => selectedUsers.Contains(u.Id));

        HttpContext.Session.SetString("SelectedUsers", JsonConvert.SerializeObject(users));
        
        return RedirectToAction("Create", "Task");
    }
    
    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectViewModel model)
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
    
