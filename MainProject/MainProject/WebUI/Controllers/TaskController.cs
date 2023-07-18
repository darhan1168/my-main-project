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
    
    public async Task<ViewResult> TaskMenu(string username)
    {
        var user = await _userService.GetUserByUsername(username);
        User = user;
        
        return View(user);
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