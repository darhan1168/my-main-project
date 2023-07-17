using Core.Enums;

namespace WebUI.Models;

public class UserRegistrationModel
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public UserRole Role { get; set; }
}