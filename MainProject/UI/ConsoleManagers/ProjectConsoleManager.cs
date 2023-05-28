using BLL.Abstraction.Interfaces;
using Core;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class ProjectConsoleManager : ConsoleManager<IProjectService, Project>, IConsoleManager<Project>
{
    private readonly TaskConsoleManager _taskConsoleManager;
    private readonly UserConsoleManager _userConsoleManager;
    private User _user;
    
    public ProjectConsoleManager(IProjectService projectService, UserConsoleManager userConsoleManager, TaskConsoleManager taskConsoleManager) 
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
        };

        while (true)
        {
            Console.WriteLine("\nProject operations:");
            Console.WriteLine("1. Display all project"); //all
            Console.WriteLine("2. Create a new project"); //steak
            Console.WriteLine("3. Update a project"); //all
            Console.WriteLine("4. Delete a project"); //steak
            Console.WriteLine("5. Exit");

            Console.Write("Enter the operation number: ");
            string input = Console.ReadLine();

            if (input == "5")
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
                Console.WriteLine($"{index} - Title: {project.Title}, Description: {project.Description}");
                Console.WriteLine("Tasks:");
                DisplayAllTask(project);
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
    public void GetUser(Guid userId)
    {
        _user = _userConsoleManager.GetById(userId);
    }
}