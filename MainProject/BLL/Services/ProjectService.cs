using BLL.Abstraction.Interfaces;
using Core;
using Core.Enums;
using DAL;
using DAL.Abstraction.Interfaces;
using Task = Core.Task;

namespace BLL;

public class ProjectService : GenericService<Project>, IProjectService
{
    private readonly IUserService _userService;
    private readonly ITaskService _taskService;
    public ProjectService(IRepository<Project> repository, IUnitOfWork unitOfWork, IUserService userService, ITaskService taskService) :
        base(repository, unitOfWork)
    {
        _userService = userService;
        _taskService = taskService;
    }
    
    public void CreateProject(Project project)
    {
        try
        {
            Add(project);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create {project.Title}. Exception: {ex.Message}");
        }
    }

    public void UpdateTitle(Guid projectId, string newTitle)
    {
        try
        {
            var project = GetById(projectId);
            if (project is null)
            {
                throw new Exception("Task not found");
            }

            project.Title = newTitle;
            Update(projectId, project);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update {newTitle} in project by {projectId}. Exception: {ex.Message}");
        }
    }

    public void UpdateDescription(Guid projectId, string newDescription)
    {
        try
        {
            var project = GetById(projectId);
            if (project is null)
            {
                throw new Exception("Task not found");
            }

            project.Description = newDescription;
            Update(projectId, project);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update {newDescription} in project by {projectId}. Exception: {ex.Message}");
        }
    }

    public async void UpdateTasks(Guid projectId, List<Task> newTasks)
    {
        try
        {
            var project = GetById(projectId);
            if (project is null)
            {
                throw new Exception("Task not found");
            }

            List<Task> tasks = new List<Task>();
            foreach (var task in project.Tasks)
            {
                var taskService = await _taskService.GetByPredicate(t => t.Id.Equals(task.Id));
                if (taskService is not null)
                {
                    tasks.Add(taskService);
                }
            }
            
            foreach (var task in newTasks)
            {
                tasks.Add(task);
            }
            
            project.Tasks = tasks;
            Update(projectId, project);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update {newTasks} in project by {projectId}. Exception: {ex.Message}");
        }
    }

    public async void UpdateUsers(Guid projectId, List<User> newUsers)
    {
        var project = GetById(projectId);
        if (project is null)
        {
            throw new Exception("Task not found");
        }
        
        List<User> users = new List<User>();
        foreach (var user in project.Users)
        {
            var specialUser = await _userService.GetByPredicate(u => u.Id.Equals(user.Id));
            users.Add(specialUser);
        }
            
        foreach (var user in newUsers)
        {
            users.Add(user);
        }
        
        project.Users = users;
        Update(projectId, project);
    }

    public void DeleteProject(Guid projectId)
    {
        try
        {
            Delete(projectId);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to delete project by {projectId}. Exception: {ex.Message}");
        }
    }

    public async Task<double> GetCompletionRate(Guid projectId)
    {
        try
        {
            var project = GetById(projectId);
            List<Task> tasks = new List<Task>();
            foreach (var task in project.Tasks)
            {
                var taskService = await _taskService.GetByPredicate(t => t.Id.Equals(task.Id));
                if (taskService is not null)
                {
                    tasks.Add(taskService);
                }
            }
            
            int completedTasksCount = tasks.Count(t => t.TaskProgress.Equals(TaskProgress.Completed));
            int totalTasksCount = tasks.Count;
            double completionPercentage = (completedTasksCount / (double)totalTasksCount) * 100;
            
            project.CompletionRate = completionPercentage;
            Update(projectId, project);
            return completionPercentage;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get completion rate. Exception: {ex.Message}");
        }
    }
}