using BLL;
using BLL.Abstraction.Interfaces;
using Core;
using Core.Enums;
using UI.Interfaces;
using Task = Core.Task;

namespace UI.ConsoleManagers;

public class TaskConsoleManager : ConsoleManager<ITaskService, Task>, IConsoleManager<Task>
{
    private readonly ProjectConsoleManager _projectConsoleManager;
    private readonly UserConsoleManager _userConsoleManager;
    private User _user;
    
    public TaskConsoleManager(ITaskService taskService, UserConsoleManager userConsoleManager, ProjectConsoleManager projectConsoleManager) 
        : base(taskService)
    {
        _userConsoleManager = userConsoleManager;
        _projectConsoleManager = projectConsoleManager;
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
            Console.Write("Enter title:");
            string title = Console.ReadLine();
            
            Console.Write("Enter description");
            string description = Console.ReadLine();
         
            var deadline = GetDeadline();

            TaskPriority priority = GetTaskPriority();

            Task task = new Task()
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = description,
                Deadline = deadline,
                TaskPriority = priority,
                TaskProgress = TaskProgress.NotStarted,
                User = _user,
                MakerRole = UserRole.Developer
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
            var projects = _projectConsoleManager.GetAll()
                .Where(p => p.Users.Any(u => u.Username.Equals(_user.Username)))
                .ToList();
            if (projects.Count == 0)
            {
                throw new Exception("Projects not added yet");
            }

            //display projects 
            Console.Write("Choose project");
            int inputProject = Int32.Parse(Console.ReadLine());
            var project = projects[inputProject - 1];
            
            DisplayAllTasks();
            var tasks = _service.GetAll().Where(t => t.User.Username.Equals(_user.Username)).ToList();
            Console.Write("Choose task to assign");
            int inputTask = Int32.Parse(Console.ReadLine());
            var task = tasks[inputTask - 1];
            
            project.Tasks.Add(task);
            task.TaskProgress = TaskProgress.InProgress;
            _service.Update(task.Id, task);
            _projectConsoleManager.Update(project.Id, project);
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
            Console.Clear();
            DisplayAllTasks();
            var tasks = _service.GetAll().Where(t => t.User.Username.Equals(_user.Username)).ToList();
            Console.Write("Choose task to update:");
            int inputTask = Int32.Parse(Console.ReadLine());
            var task = tasks[inputTask - 1];
            while (true)
            {
                Console.WriteLine("\nUpdate operations:");
                Console.WriteLine("1. Update maker");
                Console.WriteLine("2. Update title");
                Console.WriteLine("3. Update description");
                Console.WriteLine("4. Update deadline");
                Console.WriteLine("5. Update task priority");
                Console.WriteLine("6. Exit");
                
                Console.Write("Enter the operation number: ");
                string input = Console.ReadLine();
            
                if (input == "6")
                {
                    break;
                }
            
                Console.Clear();
                switch (input)
                {
                    case "1":
                        var makerRole = _userConsoleManager.GetUserRole();
                        _service.UpdateMaker(task.Id, makerRole);
                        break;
                    case "2":
                        Console.Write("Enter new title:");
                        string title = Console.ReadLine();
                        _service.UpdateTitle(task.Id, title);
                        break;
                    case "3":
                        Console.Write("Enter new description");
                        string description = Console.ReadLine();
                        _service.UpdateDescription(task.Id, description);
                        break;
                    case "4":
                        var deadline = GetDeadline();
                        _service.UpdateDeadline(task.Id, deadline);
                        break;
                    case "5":
                        TaskPriority priority = GetTaskPriority();
                        _service.UpdateTaskPriority(task.Id, priority);
                        break;
                    default:
                        Console.WriteLine("Invalid operation number."); 
                        break;
                }
                Console.WriteLine("Task successfully updated");
            }
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

    private DateTime GetDeadline()
    {
        Console.Write("Enter the deadline (yyyy-MM-dd HH:mm:ss): ");
        string deadlineInput = Console.ReadLine();

        DateTime deadline;
        if (!DateTime.TryParse(deadlineInput, out deadline))
        {
            throw new Exception("Invalid deadline format. Please enter a valid datetime value.");
        }

        return deadline;
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

    private User ChooseMaker()
    {
        var developers = _userConsoleManager.GetAll().Where(u => u.Role.Equals(UserRole.Developer)).ToList();
        if (developers.Count == 1)
        {
            throw new Exception("Developers not found");
        }

        int index = 1;
        foreach (var developer in developers)
        {
            Console.WriteLine($"{index} - Username: {developer.Username}");
            index++;
        }
        
        Console.Write("Choose task to assign");
        int input = Int32.Parse(Console.ReadLine());
        return developers[input - 1];
    }
}