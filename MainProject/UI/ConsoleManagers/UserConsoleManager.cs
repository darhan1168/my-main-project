using System.Text.RegularExpressions;
using BLL;
using BLL.Abstraction.Interfaces;
using Core;
using Core.Enums;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class UserConsoleManager : ConsoleManager<IUserService, User>, IConsoleManager<User>
{
    public UserConsoleManager(IUserService userService) : base(userService)
    {
    }

    public bool IsLogIn { get; set; } 

    public override void PerformOperations()
    {
        Dictionary<string, Action> actions = new Dictionary<string, Action>
        {
            { "1", SignUp },
            { "2", LogIn }
        };

        while (true)
        {
            Console.WriteLine("\nUser operations:");
            Console.WriteLine("1. Sign up");
            Console.WriteLine("2. Log in");
            Console.WriteLine("3. Exit");

            Console.Write("Enter the operation number: ");
            string input = Console.ReadLine();

            if (input == "3")
            {
                Environment.Exit(0);
            }

            if (actions.ContainsKey(input))
            {
                actions[input]();
                break;
            }
            else
            {
                Console.WriteLine("Invalid operation number.");
            }
        }
    }

    public void SignUp()
    {
        try
        {
            Console.Clear();
            var users = _service.GetAll();
            
            Console.WriteLine("Enter your username");
            string username = Console.ReadLine();
            if (users.Where(u => u.Username == username).ToList().Count > 0)
            {
                throw new Exception("This username already use");
            }

            Console.WriteLine("Enter your email");
            string email = Console.ReadLine();
            if (users.Where(u => u.Email == email).ToList().Count > 0)
            {
                throw new Exception("This email already use");
            }
            if (!IsValidEmail(email))
            {
                throw new Exception("Incorrect email");
            }

            Console.WriteLine("Enter your password (more then 5 symbols)");
            string password = Console.ReadLine();
            if (!IsCorrectPassword(password))
            {
                throw new Exception("Incorrect password");
            }
            
            _service.Registration(new User()
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email,
                PasswordHash = password,
                Role = GetUserRole()
            });
            Console.WriteLine("You successfully sign up, you need to log in");
        }
        catch (Exception ex)
        {
           Console.WriteLine($"Failed to sign up. Exception: {ex.Message}");
        }
    }

    public void LogIn()
    {
        try
        {
            Console.Clear();
            var users = _service.GetAll();
            
            Console.WriteLine("Enter your username");
            string username = Console.ReadLine();
            if (users.Where(u => u.Username == username).ToList().Count == 0)
            {
                throw new Exception("This username never used");
            }
            
            Console.WriteLine("Enter your password");
            string password = Console.ReadLine();

            var user = _service.Authorization(username, password);
            if (user is not null)
            {
                IsLogIn = true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to log in. Exception: {ex.Message}");
        }
    }
    
    private bool IsValidEmail(string email)
    {
        string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
        Regex regex = new Regex(pattern);
        return regex.IsMatch(email);
    }

    private bool IsCorrectPassword(string password)
    {
        return password.Length >= 6;
    }

    private UserRole GetUserRole()
    {
        try
        {
            Dictionary<string, UserRole> roles = new Dictionary<string, UserRole>()
            {
                { "1", UserRole.Developer },
                { "2", UserRole.Tester },
                { "3", UserRole.Stakeholder }
            };
            Console.WriteLine("Choose user role:");
            Console.WriteLine("1 - Developer (can do project)");
            Console.WriteLine("2 - Tester (need to test project after developers)");
            Console.WriteLine("3 - Stakeholder (check project, create tasks)");
            var input = Console.ReadLine();

            return roles[input];
        }
        catch (Exception ex)
        {
           throw new Exception($"Failed to get user role. Exception: {ex.Message}");
        }
    }
}