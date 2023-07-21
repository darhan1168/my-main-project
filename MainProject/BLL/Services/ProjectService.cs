using BLL.Abstraction.Interfaces;
using Core;
using Core.Enums;
using DAL;
using DAL.Abstraction.Interfaces;
using Task = System.Threading.Tasks.Task;
using TaskProject = Core.Task;

namespace BLL;

public class ProjectService : GenericService<Project>, IProjectService
{
    private readonly IUserService _userService;
    private readonly ITaskService _taskService;
    private readonly IUserProjectService _userProjectService;
    public ProjectService(IRepository<Project> repository, IUserService userService, 
        ITaskService taskService, IUserProjectService userProjectService) :
        base(repository)
    {
        _userService = userService;
        _taskService = taskService;
        _userProjectService = userProjectService;
    }
    
    public async Task CreateProject(Project project)
    {
        try
        {
            await Add(project);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create {project.Title}. Exception: {ex.Message}");
        }
    }
    
    public async Task UpdateAll(Guid projectId, Project newProject)
    {
        try
        {
            var project = await GetById(projectId);
            if (project is null)
            {
                throw new Exception("project not found");
            }

            project.Title = newProject.Title;
            project.Description = newProject.Description;
            await Update(projectId, project);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update {newProject.Title}. Exception: {ex.Message}");
        }
    }

    public async Task UpdateTitle(Guid projectId, string newTitle)
    {
        try
        {
            var project = await GetById(projectId);
            if (project is null)
            {
                throw new Exception("Task not found");
            }

            project.Title = newTitle;
            await Update(projectId, project);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update {newTitle} in project by {projectId}. Exception: {ex.Message}");
        }
    }

    public async Task UpdateDescription(Guid projectId, string newDescription)
    {
        try
        {
            var project = await GetById(projectId);
            if (project is null)
            {
                throw new Exception("Task not found");
            }

            project.Description = newDescription;
            await Update(projectId, project);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update {newDescription} in project by {projectId}. Exception: {ex.Message}");
        }
    }

    public async Task UpdateTasks(Guid projectId, List<TaskProject> newTasks)
    {
        try
        {
            var project = await GetById(projectId);
            if (project is null)
            {
                throw new Exception("Task not found");
            }

            List<TaskProject> tasks = new List<TaskProject>();
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
            await Update(projectId, project);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update {newTasks} in project by {projectId}. Exception: {ex.Message}");
        }
    }

    public async Task UpdateUsers(Guid projectId, List<UserProject> newUsers)
    {
        var project = await GetById(projectId);
        if (project is null)
        {
            throw new Exception("Task not found");
        }
        
        List<UserProject> users = new List<UserProject>();
        foreach (var user in project.UserProjects)
        {
            var specialUser = await _userProjectService.GetByPredicate(u => u.Id.Equals(user.Id));
            users.Add(specialUser);
        }
            
        foreach (var user in newUsers)
        {
            users.Add(user);
        }
        
        project.UserProjects = users;
        await Update(projectId, project);
    }

    public async Task DeleteProject(Guid projectId)
    {
        try
        {
            var project = await GetById(projectId, "Tasks,UserProjects");
            
            if (project == null)
            {
                throw new Exception("Project not found");
            }
            
            await Delete(projectId);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to delete project by {projectId}. Exception: {ex.Message}");
        }
    }

    public async Task UpdateCompletionRate(Guid projectId)
    {
        try
        {
            var project = await GetById(projectId);
            
            List<TaskProject> tasks = new List<TaskProject>();
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
            await Update(projectId, project);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get completion rate. Exception: {ex.Message}");
        }
    }

    public async Task SentEmail(Guid projectId, Guid taskId, string massage)
    {
        var project = await GetById(projectId,"Tasks.Files,UserProjects.User");
        var task = await _taskService.GetById(taskId);
        
        if (project.Tasks.Count == 0)
        {
            return;
        }

        var emailService = new EmailService();
        
        foreach (var userProject in project.UserProjects)
        {
            var recipientEmail = $"{userProject.User.Email}";
            var subject = $"Your task {task.Title}";
            var body = massage;
            await emailService.SendEmailAsync(recipientEmail, subject, body);
        }
    }

    public async Task CheckDeadline(List<Project> projects)
    {
        DateTime today = DateTime.Today; 
    
        foreach (var project in projects)
        {
            foreach (var task in project.Tasks)
            {
                if (task.Deadline <= today)
                {
                    await SentEmail(project.Id, task.Id, $"Your deadline {task.Deadline} finished");
                }
            }
        }
    }

    // public async Task AddUserProject(User user, Project project)
    // {
    //     await _userProjectService.CreateUserProject(user, project);
    // }
}