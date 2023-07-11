using System.Linq.Expressions;
using BLL.Abstraction.Interfaces;
using Core;
using Core.Enums;
using UI.Interfaces;
using Task = System.Threading.Tasks.Task;
using TaskProject = Core.Task;

namespace UI.ConsoleManagers;

public class ProjectConsoleManager : ConsoleManager<IProjectService, Project>, IConsoleManager<Project>
{
    private readonly TaskConsoleManager _taskConsoleManager;
    private readonly UserConsoleManager _userConsoleManager;
    private User _user;
    
    public ProjectConsoleManager(IProjectService projectService, UserConsoleManager userConsoleManager, 
        TaskConsoleManager taskConsoleManager) 
        : base(projectService)
    {
        _userConsoleManager = userConsoleManager;
        _taskConsoleManager = taskConsoleManager;
    }

    public override async Task PerformOperations()
    {
        Dictionary<string, Func<Task>> actions = new Dictionary<string, Func<Task>>
        {
            { "1", DisplayAllProjects },
            { "2", CreateProject },
            { "3", UpdateProject },
            { "4", DeleteProject },
            { "5", Task },
        };

        while (true)
        {
            Console.WriteLine("\nProject operations:");
            Console.WriteLine("1. Display all project");
            Console.WriteLine("2. Create a new project");
            Console.WriteLine("3. Update a project");
            Console.WriteLine("4. Delete a project"); 
            Console.WriteLine("5. Work with my task");
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

    public async Task DisplayAllProjects()
    {
        try
        {
            Console.Clear();
            var projects = await GetAllProjects();

            int index = 1;
            foreach (var project in projects)
            {
                Console.WriteLine("-----");
                var completionRate = _service.GetCompletionRate(project.Id);
                Console.WriteLine($"{index} Project - Title: {project.Title}, Description: {project.Description}, Completion rate: {completionRate} %");
                
                Console.WriteLine("Tasks:");
                DisplayAllTask(project);
                
                Console.WriteLine("Makers:");
                DisplayAllMakers(project);
                index++;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to display all projects. Exception: {ex.Message}");
        }
    }

    public async Task CreateProject()
    {
        try
        {
            IsSteakHolder("create");
            Console.Clear();
            Console.Write("Enter title:");
            string title = Console.ReadLine();
            
            Console.Write("Enter description:");
            string description = Console.ReadLine();

            var user = await GetUsers();
            var tasks = await GetTasks();

            var project = new Project()
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = description,
                Tasks = tasks
            };

            await _service.CreateProject(project);
            await _service.AddUserProject(user.FirstOrDefault(), project);
            Console.WriteLine("Your project successfully added");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to create project. Exception: {ex.Message}");
        }
    }
    
    public async Task DeleteProject()
    {
        try
        {
            IsSteakHolder("delete");
            var projects = await GetAllProjects();
            await DisplayAllProjects();
            
            Console.WriteLine("Enter number of project for deleting");
            int input = Int32.Parse(Console.ReadLine());
            
            await _service.DeleteProject(projects[input - 1].Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to delete project. Exception: {ex.Message}");
        }
    }
    
    public async Task UpdateProject()
    {
        try
        {
            IsSteakHolder("update");
            Console.Clear();
            var project = await GetProject("update");
            while (true)
            {
                Console.WriteLine("\nUpdate operations:");
                Console.WriteLine("1. Update title");
                Console.WriteLine("2. Update description");
                Console.WriteLine("3. Add Tasks");
                Console.WriteLine("4. Add Users");
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
                        await _service.UpdateTitle(project.Id, title);
                        break;
                    case "2":
                        Console.Write("Enter new description");
                        string description = Console.ReadLine();
                        await _service.UpdateDescription(project.Id, description);
                        break;
                    case "3":
                        var tasks = await GetTasks();
                        await _service.UpdateTasks(project.Id, tasks);
                        break;
                    case "4":
                        // var users = await GetUsers();
                        // _service.UpdateUsers(project.Id, users);
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
            Console.WriteLine($"Failed to update project. Exception: {ex.Message}");
        }
    }

    private async Task<List<Project>> GetAllProjects()
    {
        var projects = await GetListByPredicate(p => p.UserProjects.Any(u => u.UserId.Equals(_user.Id)));
        if (projects.Count == 0)
        {
            throw new Exception("Projects not added yet");
        }

        return projects;
    }

    private async Task DisplayAllTask(Project project)
    {
        List<TaskProject> tasks = new List<TaskProject>();
        foreach (var task in project.Tasks)
        {
            var taskService = await _taskConsoleManager.GetByPredicate(t => t.Id.Equals(task.Id));
            if (taskService is not null)
            {
                tasks.Add(taskService);
            }
        }
        
        if (tasks.Count == 0)
        {
            Console.WriteLine("Task not added yet");
        }

        int index = 1;
        foreach (var task in tasks)
        {
            Console.WriteLine($"{index} - Title: {task.Title}, Description: {task.Description}, Deadline: {task.Deadline}, " +
                              $"Progress: {task.TaskProgress}, Priority: {task.TaskPriority}");
            index++;
        }
    }
    
    private async Task DisplayAllMakers(Project project)
    {
        var userProjects = project.UserProjects;
        if (userProjects.Count == 0)
        {
            throw new Exception("userProjects not added yet");
        }
        
        int index = 1;
        foreach (var userProject in userProjects)
        {
            var user = await _userConsoleManager.GetById(userProject.UserId);
            
            Console.WriteLine($"{index} - Username: {user.Username}, Role: {user.Role}");
            index++;
        }
    }

    private async Task<List<User>> GetUsers()
    {
        List<User> users = new List<User>();
        users.Add(_user);
        while (true)
        {
            Console.WriteLine("Enter username, which need to add");
            var username = Console.ReadLine();
            var user = await _userConsoleManager.GetByPredicate(u => u.Username.Equals(username));
            if (users.Contains(user))
            {
                Console.WriteLine("This user already added");
                continue;
            }
            
            users.Add(user);
            Console.WriteLine($"Username: {user.Username}, Role: {user.Role} - will add in project");
            
            Console.WriteLine("Enter 1 if you want to add more");
            string answer = Console.ReadLine();
            if (answer == "1")
            {
                continue;
            }

            return users;
        }
    }

    private async Task<List<TaskProject>> GetTasks()
    {
        List<TaskProject> tasks = new List<TaskProject>();
        while (true)
        {
            var task = await _taskConsoleManager.GetTask("add in project");
            if (tasks.Contains(task))
            {
                Console.WriteLine("This task already added");
                continue;
            }

            if (task.TaskProgress == TaskProgress.NotStarted)
            {
                task.TaskProgress = TaskProgress.InProgress;
                await _taskConsoleManager.Update(task.Id, task);
            }
            
            tasks.Add(task);
            Console.WriteLine($"Your tasks will add in project");
            
            Console.WriteLine("Enter 1 if you want to add more");
            string answer = Console.ReadLine();
            if (answer == "1")
            {
                continue;
            }

            return tasks;
        }
    }
    
    private void IsSteakHolder(string reason)
    {
        if (!_user.Role.Equals(UserRole.Stakeholder))
        {
            throw new Exception($"Only stakeholder can {reason} projects");
        }
    }
 
    public async Task GetUser(Guid userId)
    {
        _user = await _userConsoleManager.GetById(userId);
    }
    
    private async Task<Project> GetProject(string cause)
    {
        await DisplayAllProjects();
        var projects = await GetAllProjects();
        if (projects.Count == 0)
        {
            throw new Exception("Projects not added yet");
        }
        
        Console.Write($"Choose project to {cause}:");
        int input = Int32.Parse(Console.ReadLine());
        return projects[input - 1];
    }

    public async Task Task()
    {
        try
        {
            await DisplayAllProjects();
            var projects = await GetAllProjects();
            
            Console.Write("Choose project:");
            int input = Int32.Parse(Console.ReadLine());
            
            await _taskConsoleManager.GetProject(projects[input - 1]);
            await _taskConsoleManager.GetUser(_userConsoleManager.User.Id);
            await _taskConsoleManager.PerformOperations();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to work with task. Exception: {ex.Message}");
        }
    }
}