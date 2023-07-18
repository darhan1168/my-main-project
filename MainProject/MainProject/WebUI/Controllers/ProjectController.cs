using BLL.Abstraction.Interfaces;
using Core;
using Microsoft.AspNetCore.Mvc;
using WebUI.Models;

namespace WebUI.Controllers;

public class ProjectController : Controller
{
    private readonly IProjectService _projectService;
    private readonly IUserService _userService;

    public ProjectController(IProjectService projectService, IUserService userService)
    {
        _projectService = projectService;
        _userService = userService;
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
    
