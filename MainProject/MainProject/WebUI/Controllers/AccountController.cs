using BLL;
using BLL.Abstraction.Interfaces;
using Core;
using Microsoft.AspNetCore.Mvc;
using WebUI.Models;
using Task = System.Threading.Tasks.Task;

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
    public async Task<ActionResult> Register(UserRegistrationModel model)
    {
        try
        {
            bool hasErrors = false;
        
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password) || string.IsNullOrWhiteSpace(model.Email))
            {
                ViewData["GeneralError"] = "Please all fields must be completed";
                return View(model);
            }
        
            if (!await _userService.IsValuableUsername(model.Username))
            {
                ViewData["UsernameError"] = "This username already used";
                hasErrors = true;
            }
        
            if (!await _userService.IsValidPassword(model.Password))
            {
                ViewData["PasswordError"] = "Password should consist of 6 symbols";
                hasErrors = true;
            }

            if (!_userService.IsValidEmail(model.Email))
            {
                ViewData["EmailError"] = "This email is not correct";
                hasErrors = true;
            }
            
            if (!(await _userService.IsEmailAvailable(model.Email)))
            {
                ViewData["EmailError"] = "This email are almost occupied";
                hasErrors = true;
            }

            if (hasErrors)
            {
                return View(model);
            }
        
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = model.Password,
                Role = model.Role,
                UserProjects = new List<UserProject>()
            };
        
            await _userService.Registration(user);
    
            return RedirectToAction("Login", "Account");
        }
        catch (Exception e)
        {
            ViewData["GeneralError"] = $"Error : {e}";
            return View(model);
        }
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
                if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
                {
                    ViewData["LoginError"] = "Please all fields must be completed";
                    return View(model);
                }
                
                var user = await _userService.Authorization(model.Username, model.Password);
            
                if (user != null)
                {
                    IsLogIn = true;
                    User = await _userService.GetUserByUsername(model.Username);
                    return RedirectToAction("Index", "Project");
                }
                else
                {
                    ViewData["LoginError"] = "Incorrect username or password";
                    return View(model);
                }
            }
        
            return View(model);
        }
        catch (Exception e)
        {
            ViewData["LoginError"] = $"Error: {e.Message}";
            return View(model);
        }
    }
}