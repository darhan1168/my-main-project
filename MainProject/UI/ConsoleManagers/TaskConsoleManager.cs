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
    
    public void DisplayAllTasks()
    {
        try
        {
            Console.Clear();
            var tasks = _service.GetAll().Where(t => t.User.Username.Equals(_user.Username)).ToList();
            if (tasks.Count == 0)
            {
                throw new Exception("Task not added yet");
            }

            int index = 1;
            foreach (var task in tasks)
            {
                Console.WriteLine($"{index} - Title: {task.Title}, Description: {task.Description}, Deadline: {task.Deadline}, " +
                                  $"Progress: {task.TaskProgress}, Priority: {task.TaskPriority}");
                index++;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to display all tasks. Exception: {ex.Message}");
        }
    }
    
    public void CreateTask()
    {
        try
        {
            Console.Clear();
            Console.WriteLine("Enter title");
            Console.Write("Answer:");
            string title = Console.ReadLine();
            
            Console.WriteLine("Enter description");
            Console.Write("Answer:");
            string description = Console.ReadLine();
            
            Console.Write("Enter the deadline (yyyy-MM-dd HH:mm:ss): ");
            Console.Write("Answer:");
            string deadlineInput = Console.ReadLine();

            DateTime deadline;
            if (!DateTime.TryParse(deadlineInput, out deadline))
            {
                throw new Exception("Invalid deadline format. Please enter a valid datetime value.");
            }

            TaskPriority priority = GetTaskPriority();

            Task task = new Task()
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = description,
                Deadline = deadline,
                TaskPriority = priority,
                TaskProgress = TaskProgress.NotStarted,
                User = _user
            };
            _service.CreateTask(task);
            Console.WriteLine("Your tasks successfully added");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to create task. Exception: {ex.Message}");
        }
    }
    
    public void AssignTask()
    {
        try
        {
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to create task. Exception: {ex.Message}");
        }
    }
    
    public void UpdateTask()
    {
        try
        {
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to update task. Exception: {ex.Message}");
        }
    }
    
    public void DeleteTask()
    {
        try
        {
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to delete task. Exception: {ex.Message}");
        }
    }

    public void GetUser(Guid userId)
    {
        _user = _userConsoleManager.GetById(userId);
    }

    private TaskPriority GetTaskPriority()
    {
        Console.WriteLine("Choose priority");
        Console.WriteLine("1 - Low");
        Console.WriteLine("2 - Normal");
        Console.WriteLine("3 - High");
        Console.WriteLine("4 - Urgent");
        Console.WriteLine("5 - Critical");
        Console.Write("Enter the priority number: ");
        string input = Console.ReadLine();
        Dictionary<string, TaskPriority> priorities = new Dictionary<string, TaskPriority>()
        {
            { "1", TaskPriority.Low },
            { "2", TaskPriority.Normal },
            { "3", TaskPriority.High },
            { "4", TaskPriority.Urgent },
            { "5", TaskPriority.Critical }
        };

        return priorities[input];
    }
}