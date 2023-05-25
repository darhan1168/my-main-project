using BLL;
using BLL.Abstraction.Interfaces;
using Core;
using Core.Enums;
using UI.Interfaces;
using Task = Core.Task;

namespace UI.ConsoleManagers;

public class TaskConsoleManager : ConsoleManager<ITaskService, Task>, IConsoleManager<Task>
{
    private readonly UserConsoleManager _userConsoleManager;
    private User _user;
    
    public TaskConsoleManager(ITaskService taskService, UserConsoleManager userConsoleManager) 
        : base(taskService)
    {
        _userConsoleManager = userConsoleManager;
    }

    public override void PerformOperations()
    {
        Dictionary<string, Action> actions = new Dictionary<string, Action>
        {
            { "1", DisplayAllTasks },
            { "2", CreateTask },
            { "3", AssignTask },
            { "4", UpdateTask },
            { "5", DeleteTask },
        };

        while (true)
        {
            if (!_user.Role.Equals(UserRole.Stakeholder))
            {
                Console.WriteLine("Only stakeholder can create and work with tasks");
                break;
            }
            
            Console.WriteLine("\nUser operations:");
            Console.WriteLine("1. Display all your tasks");
            Console.WriteLine("2. Create a new task");
            Console.WriteLine("3. Assign a task");
            Console.WriteLine("4. Update a task");
            Console.WriteLine("5. Delete a task");
            Console.WriteLine("6. Exit");

            Console.Write("Enter the operation number: ");
            string input = Console.ReadLine();

            if (input == "6")
            {
                break;
            }

            if (actions.ContainsKey(input))
            {
                actions[input]();
            }
            else
            {
                Console.WriteLine("Invalid operation number.");
            }
        }
    }

    public void GetUser(Guid userId)
    {
        _user = _userConsoleManager.GetById(userId);
    }
