using Core.Enums;
using UI.ConsoleManagers;
using Task = System.Threading.Tasks.Task;

namespace UI;

public class AppManager
{
    private readonly ProjectConsoleManager _projectConsoleManager;
    private readonly TaskConsoleManager _taskConsoleManager;
    private readonly UserConsoleManager _userConsoleManager;

    public AppManager(ProjectConsoleManager projectConsoleManager,
        TaskConsoleManager taskConsoleManager,
        UserConsoleManager userConsoleManager)
    {
        _projectConsoleManager = projectConsoleManager;
        _taskConsoleManager = taskConsoleManager;
        _userConsoleManager = userConsoleManager;
    }

    public async Task Start()
    {
        while (true)
        {
            await _userConsoleManager.PerformOperations();
        
            while (true)
            {
                if (_userConsoleManager.IsLogIn && _userConsoleManager.User.Role.Equals(UserRole.Stakeholder))
                {
                    await _taskConsoleManager.GetUser(_userConsoleManager.User.Id);
                    await _projectConsoleManager.GetUser(_userConsoleManager.User.Id);
                    
                    Console.WriteLine("\nChoose an operation:");
                    Console.WriteLine("1. Task operations");
                    Console.WriteLine("2. Project operations");
                    Console.WriteLine("3. Exit");
                
                    Console.Write("Enter the operation number: ");
                    string input = Console.ReadLine();
                
                    switch (input)
                    {
                        case "1":
                            await _taskConsoleManager.PerformOperations();
                            break;
                        case "2":
                            await _projectConsoleManager.PerformOperations();
                            break;
                        case "3":
                            return;
                        default:
                            Console.WriteLine("Invalid operation number.");
                            break;
                    }
                }
                else if (_userConsoleManager.IsLogIn)
                {
                    Console.WriteLine("1. Project operations");
                    Console.WriteLine("2. Exit");
                
                    Console.Write("Enter the operation number: ");
                    string input = Console.ReadLine();
                
                    switch (input)
                    {
                        case "1":
                            await _projectConsoleManager.GetUser(_userConsoleManager.User.Id);
                            await _projectConsoleManager.PerformOperations();
                            break;
                        case "2":
                            return;
                        default:
                            Console.WriteLine("Invalid operation number.");
                            break;
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}