using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;

namespace Core;

public class User : BaseEntity
{ 
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public UserRole Role { get; set; }
    public List<UserProject> UserProjects { get; set; }
}