using BLL.Abstraction.Interfaces;
using Core;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers;

public class TaskController : Controller
{
    private readonly IUserService _userService;

    public User User { get; set; }
    
    public TaskController(IUserService userService)
    {
        _userService = userService;
    }
    
    public async Task<ViewResult> TaskMenu(string username)
    {
        var user = await _userService.GetUserByUsername(username);
        User = user;
        
        return View(user);
    }
}