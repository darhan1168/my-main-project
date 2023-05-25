using UI.ConsoleManagers;

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

    public void Start()
    {
        while (true)
        {
            _userConsoleManager.PerformOperations();
        
            while (true)
            {
                if (_userConsoleManager.IsLogIn)
                {
                    Console.WriteLine("\nChoose an operation:");
                    Console.WriteLine("1. Task operations");
                    Console.WriteLine("2. Project operations");
                    Console.WriteLine("3. Exit");
                
                    Console.Write("Enter the operation number: ");
                    string input = Console.ReadLine();
                
                    switch (input)
                    {
                        case "1":
                            _taskConsoleManager.PerformOperations();
                            break;
                        case "2":
                            _projectConsoleManager.PerformOperations();
                            break;
                        case "3":
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