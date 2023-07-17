using BLL;
using BLL.Abstraction.Interfaces;
using Core;
using Microsoft.AspNetCore.Mvc;
using WebUI.Models;

namespace WebUI.Controllers;

public class AccountController : Controller
{
    private readonly IUserService _userService;
    public bool IsLogIn { get; set; }
    
    public User User { get; set; }
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
        if (!await _userService.IsValuableUsername(model.Username))
        {
            ViewData["UsernameError"] = "This username already used";
            return View(model);
        }
        
        if (model.PasswordHash.Length < 6)
        {
            ViewData["PasswordError"] = "Password should consist of 6 symbols";
            return View(model);
        }
        
        var user = new User
        {
            Username = model.Username,
            Email = model.Email,
            PasswordHash = model.PasswordHash,
            Role = model.Role,
            UserProjects = new List<UserProject>()
        };
        
        await _userService.Registration(user);
    
        return RedirectToAction("Login", "Account");
    }
    
    [HttpGet]
    public ActionResult Login()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<ActionResult> Login(LoginViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.Authorization(model.Username, model.Password);
            
                if (user != null)
                {
                    IsLogIn = true;
                    User = await _userService.GetUserByUsername(model.Username);
                    return RedirectToAction("TaskMenu", "Task");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    return View();
                }
            }
        
            return View(model);
        }
        catch (Exception e)
        {
            ViewData["PasswordError"] = "Password should consist of 6 symbols";
            return View(model);
        }
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