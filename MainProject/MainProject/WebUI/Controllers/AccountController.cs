using BLL;
using BLL.Abstraction.Interfaces;
using Core;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers;

public class AccountController : Controller
{
    private readonly IUserService _userService;
    
    public AccountController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet]
    public ActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Register(User model)
    {
        var user = new User
        {
            Username = model.Username,
            Email = model.Email,
            PasswordHash = model.PasswordHash,
            Role = model.Role,
            UserProjects = new List<UserProject>()
        };
        
        await _userService.Registration(user);
    
        return RedirectToAction("Index", "Home");
    }
    
    // [HttpPost]
    // public async Task<ViewResult> SaveRegisterDetails(User model)
    // {
    //     if (ModelState.IsValid)
    //     {
    //         var user = new User
    //         {
    //             Username = model.Username,
    //             Email = model.Email,
    //             PasswordHash = model.PasswordHash,
    //             Role = model.Role,
    //             UserProjects = new List<UserProject>()
    //         };
    //     
    //         await _userService.Registration(user);
    //         
    //         ViewBag.Message = "User Details Saved";
    //         return View("Register");
    //     }
    //     else
    //     {
    //         
    //         return View("Register", model);
    //     }
    // }
}