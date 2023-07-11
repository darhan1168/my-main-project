using System.Linq.Expressions;
using System.Net;
using BLL;
using BLL.Abstraction.Interfaces;
using Core;
using Core.Enums;
using UI.Interfaces;
using Task = System.Threading.Tasks.Task;
using TaskProject = Core.Task;

namespace UI.ConsoleManagers;

public class TaskConsoleManager : ConsoleManager<ITaskService, TaskProject>, IConsoleManager<TaskProject>
{
    private readonly UserConsoleManager _userConsoleManager;
    private User _user;
    private Project _project;
    
    public TaskConsoleManager(ITaskService taskService, UserConsoleManager userConsoleManager) 
        : base(taskService)
    {
        _userConsoleManager = userConsoleManager;
    }

    public override async Task PerformOperations()
    {
        Dictionary<string, Func<Task>> actions = new Dictionary<string, Func<Task>>
        {
            { "1", DisplayAllTasks },
            { "2", CreateTask },
            { "3", UpdateTask },
            { "4", CheckTask },
            { "5", DeleteTask }
        };

        while (true)
        {
            Console.WriteLine("\nUser operations:");
            Console.WriteLine("1. Display all your tasks");
            Console.WriteLine("2. Create a new task");
            Console.WriteLine("3. Update a task"); 
            Console.WriteLine("4. Check a task"); 
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
                await actions[input]();
            }
            else
            {
                Console.WriteLine("Invalid operation number.");
            }
        }
    }

    public async Task DisplayAllTasks()
    {
        try
        {
            Console.Clear();
            var tasks = await _service.GetListByPredicate(t => t.Users.Any(u => u.Username.Equals(_user.Username)));
            if (tasks.Count == 0)
            {
                foreach (var task in _project.Tasks)
                {
                    var specialTasks = await _service.GetByPredicate(t => t.Id.Equals(task.Id));
                    tasks.Add(specialTasks);
                }
            }
            
            if (tasks.Count == 0)
            {
                throw new Exception("Task not added yet");
            }

            int index = 1;
            foreach (var task in tasks)
            {
                Console.WriteLine($"{index} - Title: {task.Title}, Description: {task.Description}, Deadline: {task.Deadline}, " +
                                  $"Progress: {task.TaskProgress}, Priority: {task.TaskPriority}, Files in task: {task.Files.Count}");
                index++;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to display all tasks. Exception: {ex.Message}");
        }
    }
    
    public async Task CreateTask()
    {
        try
        {
            IsSteakHolder("create");
            Console.Clear();
            Console.Write("Enter title:");
            string title = Console.ReadLine();
            
            Console.Write("Enter description:");
            string description = Console.ReadLine();
         
            var deadline = GetDeadline();
            var priority = GetTaskPriority();
            var responsibleUser = await ChooseResponsible();
            var files = GetFiles();

            var users = new List<User>()
            {
                _user,
                responsibleUser
            };
            
            TaskProject task = new TaskProject()
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = description,
                Deadline = deadline,
                TaskPriority = priority,
                TaskProgress = TaskProgress.NotStarted,
                Users = users,
                Files = files
            };
            
            await _service.CreateTask(task);
            Console.WriteLine("Your tasks successfully added");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to create task. Exception: {ex.Message}");
        }
    }

    public async Task UpdateTask()
    {
        try
        {
            IsSteakHolder("update");
            Console.Clear();
            var task = await GetTask("update");
            while (true)
            {
                Console.WriteLine("\nUpdate operations:");
                Console.WriteLine("1. Update title");
                Console.WriteLine("2. Update description");
                Console.WriteLine("3. Update deadline");
                Console.WriteLine("4. Update task priority");
                Console.WriteLine("5. Add files");
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
                        Console.Write("Enter new title:");
                        string title = Console.ReadLine();
                        _service.UpdateTitle(task.Id, title);
                        break;
                    case "2":
                        Console.Write("Enter new description");
                        string description = Console.ReadLine();
                        _service.UpdateDescription(task.Id, description);
                        break;
                    case "3":
                        var deadline = GetDeadline();
                        _service.UpdateDeadline(task.Id, deadline);
                        break;
                    case "4":
                        TaskPriority priority = GetTaskPriority();
                        _service.UpdateTaskPriority(task.Id, priority);
                        break;
                    case "5":
                        task.Files.Add(GetFile());
                        _service.Update(task.Id, task);
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

    public async Task CheckTask()
    {
        try
        {
            Console.Clear();
            var task = await GetTask("check");
            if (task.TaskProgress.Equals(TaskProgress.InProgress)
                && !task.Users.Any(u => u.Username.Equals(_user.Username))
                && _user.Role.Equals(UserRole.Developer))
            {
                TransitionNewStep("checked", task);
                Console.WriteLine("Task successfully checked and wait to test");
            }
            else if ((task.TaskProgress.Equals(TaskProgress.InProgress)
                     && task.Users.Any(u => u.Username.Equals(_user.Username))
                     && _user.Role.Equals(UserRole.Developer))
                     || _user.Role.Equals(UserRole.Tester)
                     || _user.Role.Equals(UserRole.Stakeholder))
            {
                Console.WriteLine("Task must check another developer");
            }

            if (task.TaskProgress.Equals(TaskProgress.Tested)
                && _user.Role.Equals(UserRole.Tester))
            {
                TransitionNewStep("tested", task);
                Console.WriteLine("Task successfully tested and wait to pending approval");
            }
            else if (task.TaskProgress.Equals(TaskProgress.Tested)
                     && !_user.Role.Equals(UserRole.Tester))
            {
                Console.WriteLine("Task must tested just tester");
            }
            
            if (task.TaskProgress.Equals(TaskProgress.PendingApproval)
                && _user.Role.Equals(UserRole.Stakeholder))
            {
                TransitionNewStep("done", task);
                Console.WriteLine("Task successfully completed");
            }
            else if (task.TaskProgress.Equals(TaskProgress.PendingApproval)
                     && !_user.Role.Equals(UserRole.Stakeholder))
            {
                Console.WriteLine("Task must completed just by stakeholder");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to check task. Exception: {ex.Message}");
        }
    }
    
    public async Task DeleteTask()
    {
        try
        {
            IsSteakHolder("delete");
            
            Console.Clear();
            var task = await GetTask("delete");
            
            await _service.DeleteTask(task.Id);
            Console.WriteLine("Task successfully deleted");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to delete task. Exception: {ex.Message}");
        }
    }

    public async Task GetUser(Guid userId)
    {
        _user = await _userConsoleManager.GetById(userId);
    }
    
    public async Task GetProject(Project project)
    {
        _project = project;
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

    public async Task<TaskProject> GetTask(string cause)
    {
        await DisplayAllTasks();
        var tasks = await _service.GetListByPredicate(t => t.Users.Any(u => u.Username.Equals(_user.Username)));
        if (tasks.Count == 0)
        {
            foreach (var task in _project.Tasks)
            {
                var specialTask = await _service.GetByPredicate(t => t.Id.Equals(task.Id));
                tasks.Add(specialTask);
            }
        }
        
        Console.Write($"Choose task to {cause}:");
        int inputTask = Int32.Parse(Console.ReadLine());
        return tasks[inputTask - 1];
    }
    
    private async Task<User> ChooseResponsible()
    {
        var developers = await _userConsoleManager.GetListByPredicate(u => u.Role.Equals(UserRole.Developer));
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
        
        Console.Write("Choose developer to assign:");
        int input = Int32.Parse(Console.ReadLine());
        return developers[input - 1];
    }

    private List<TaskFile> GetFiles()
    {
        List<TaskFile> files = new List<TaskFile>();
        Console.WriteLine("Enter 1 if you want to add files in task");
        string answer = Console.ReadLine();
        if (answer != "1")
        {
            return files;
        }
        
        while (true)
        {
            files.Add(GetFile());
            Console.WriteLine("File successfully added");
            Console.WriteLine("Enter 1 if you want to add more");
            string input = Console.ReadLine();
            if (input != "1")
            {
                return files;
            }
        }
    }

    private TaskFile GetFile()
    {
        while (true)
        {
            Console.Write("Enter file path:");
            string path = Console.ReadLine();
            
            Console.Write("Enter file name:");
            string name = Console.ReadLine();
            
            string fullPath = Path.Combine(path, name);
            if (File.Exists(fullPath))
            {
                var file = new TaskFile()
                {
                    Id = Guid.NewGuid(),
                    FilePath = path,
                    FileName = name,
                    CreatedBy = _user,
                    CreationDate = DateTime.Today
                };

                return file;
            }
            else
            {
                Console.WriteLine("This file doesn't look for");
            }
        }
    }

    private void IsSteakHolder(string reason)
    {
        if (!_user.Role.Equals(UserRole.Stakeholder))
        {
           throw new Exception($"Only stakeholder can {reason} tasks");
        }
    }

    private void TransitionNewStep(string action, TaskProject task)
    {
        Console.WriteLine($"Enter 1 if you already {action} task and it is correct");
        string input = Console.ReadLine();
        if (input == "1")
        {
            _service.TransitionNewStep(task.Id);
        }
        else
        {
            return;
        }
    }
}