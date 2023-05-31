using BLL.Abstraction.Interfaces;
using Core;
using Core.Enums;
using UI.Interfaces;
using Task = Core.Task;

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

    public override void PerformOperations()
    {
        Dictionary<string, Action> actions = new Dictionary<string, Action>
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
            Console.WriteLine("1. Display all project"); //all
            Console.WriteLine("2. Create a new project"); //steak
            Console.WriteLine("3. Update a project"); //all
            Console.WriteLine("4. Delete a project"); //steak
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
                actions[input]();
            }
            else
            {
                Console.WriteLine("Invalid operation number.");
            }
        }
    }
    
    public void DisplayAllProjects()
    {
        try
        {
            Console.Clear();
            var projects = GetAllProjects();

            int index = 1;
            foreach (var project in projects)
            {
                var completionRate = _service.GetCompletionRate(project.Id);
                Console.WriteLine($"{index} - Title: {project.Title}, Description: {project.Description}, Completion rate: {completionRate} %");
                
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

    public void CreateProject()
    {
        try
        {
            if (!_user.Role.Equals(UserRole.Stakeholder))
            {
                Console.WriteLine("Only stakeholder can create project");
                return;
            }
            
            Console.Clear();
            Console.Write("Enter title:");
            string title = Console.ReadLine();
            
            Console.Write("Enter description:");
            string description = Console.ReadLine();

            var users = GetUsers();
            var tasks = GetTasks();
            
            _service.CreateProject(new Project()
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = description,
                Users = users,
                Tasks = tasks
            });
            Console.WriteLine("Your project successfully added");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to create project. Exception: {ex.Message}");
        }
    }
    
    public void DeleteProject()
    {
        try
        {
            if (!_user.Role.Equals(UserRole.Stakeholder))
            {
                Console.WriteLine("Only stakeholder can delete project");
                return;
            }

            var projects = GetAllProjects();
            DisplayAllProjects();
            
            Console.WriteLine("Enter number of project for deleting");
            int input = Int32.Parse(Console.ReadLine());
            
            _service.DeleteProject(projects[input - 1].Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to delete project. Exception: {ex.Message}");
        }
    }
    
    public void UpdateProject()
    {
        try
        {
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to update project. Exception: {ex.Message}");
        }
    }

    private List<Project> GetAllProjects()
    {
        var projects = GetAll()
            .Where(p => p.Users.Any(u => u.Username.Equals(_user.Username)))
            .ToList();
        if (projects.Count == 0)
        {
            throw new Exception("Projects not added yet");
        }

        return projects;
    }

    private void DisplayAllTask(Project project)
    {
        var tasks = project.Tasks;
        if (tasks.Count == 0)
        {
            throw new Exception("Tasks not added yet");
        }

        int index = 1;
        foreach (var task in tasks)
        {
            Console.WriteLine($"{index} - Title: {task.Title}, Description: {task.Description}, Deadline: {task.Deadline}, " +
                              $"Progress: {task.TaskProgress}, Priority: {task.TaskPriority}");
            index++;
        }
    }
    
    private void DisplayAllMakers(Project project)
    {
        var users = project.Users;
        if (users.Count == 0)
        {
            throw new Exception("Users not added yet");
        }

        int index = 1;
        foreach (var user in users)
        {
            Console.WriteLine($"{index} - Username: {user.Username}, Role: {user.Role}");
            index++;
        }
    }

    private List<User> GetUsers()
    {
        List<User> users = new List<User>();
        users.Add(_user);
        while (true)
        {
            Console.WriteLine("Enter username, which need to add");
            string username = Console.ReadLine();
            User user = _userConsoleManager.GetByPredicate(u => u.Username.Equals(username));
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

    private List<Task> GetTasks()
    {
        List<Task> tasks = new List<Task>();
        while (true)
        {
            var task = _taskConsoleManager.GetTask("add in project");
            if (tasks.Contains(task))
            {
                Console.WriteLine("This task already added");
                continue;
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
 
    public void GetUser(Guid userId)
    {
        _user = _userConsoleManager.GetById(userId);
    }

    public void Task()
    {
        _taskConsoleManager.GetUser(_userConsoleManager.User.Id);
        _taskConsoleManager.PerformOperations();
    }
}