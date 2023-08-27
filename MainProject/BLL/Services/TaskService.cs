using BLL.Abstraction.Interfaces;
using DAL.Abstraction.Interfaces;
using Task = System.Threading.Tasks.Task;
using TaskProject = Core.Task;
using Core.Enums;
using Exception = System.Exception;

namespace BLL;

public class TaskService : GenericService<TaskProject>, ITaskService
{
    private readonly IUserService _userService;
    private readonly ITaskFileService _taskFileService;
    
    public TaskService(IRepository<TaskProject> repository, IUserService userService, ITaskFileService taskFileService) :
        base(repository)
    {
        _userService = userService;
        _taskFileService = taskFileService;
    }
    
    public async Task CreateTask(TaskProject task)
    {
        try
        {
            if (string.IsNullOrEmpty(task.Title) || string.IsNullOrEmpty(task.Description))
            {
                throw new Exception("Task title and description cannot be empty.");
            }
            
            if (task.Deadline < DateTime.Now)
            {
                throw new Exception("Task deadline cannot be in the past.");
            }
            
            await Add(task);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create {task.Title}. Exception: {ex.Message}");
        }
    }

    public async Task UpdateAll(Guid taskId, TaskProject newTask)
    {
        try
        {
            var task = await GetById(taskId);
            if (task is null)
            {
                throw new Exception("Task not found");
            }
            
            task.TaskPriority = newTask.TaskPriority;
            task.Title = newTask.Title;
            task.Deadline = newTask.Deadline;
            task.Description = task.Description;
            
            await Update(taskId, task);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update {newTask.Title}. Exception: {ex.Message}");
        }
    }

    public async Task UpdateTitle(Guid taskId, string newTitle)
    {
        try
        {
            var task = await GetById(taskId);
            if (task is null)
            {
                throw new Exception("Task not found");
            }

            task.Title = newTitle;
            await Update(taskId, task);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update {newTitle} in task by {taskId}. Exception: {ex.Message}");
        }
    }

    public async Task UpdateDescription(Guid taskId, string newDescription)
    {
        try
        {
            var task = await GetById(taskId);
            if (task is null)
            {
                throw new Exception("Task not found");
            }

            task.Description = newDescription;
            await Update(taskId, task);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update {newDescription} in task by {taskId}. Exception: {ex.Message}");
        }
    }

    public async Task UpdateDeadline(Guid taskId, DateTime newDeadline)
    {
        try
        {
            var task = await GetById(taskId);
            if (task is null)
            {
                throw new Exception("Task not found");
            }

            task.Deadline = newDeadline;
            await Update(taskId, task);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update {newDeadline.ToShortDateString()} in task by {taskId}. Exception: {ex.Message}");
        }
    }

    public async Task UpdateTaskPriority(Guid taskId, TaskPriority newTaskPriority)
    {
        try
        {
            var task = await GetById(taskId);
            if (task is null)
            {
                throw new Exception("Task not found");
            }

            task.TaskPriority = newTaskPriority;
            await Update(taskId, task);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update {newTaskPriority} in task by {taskId}. Exception: {ex.Message}");
        }
    }

    public async Task UpdateTaskProgress(Guid taskId, TaskProgress newTaskProgress)
    {
        try
        {
            var task = await GetById(taskId);
            if (task is null)
            {
                throw new Exception("Task not found");
            }

            task.TaskProgress = newTaskProgress;
            await Update(taskId, task);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update {newTaskProgress} in task by {taskId}. Exception: {ex.Message}");
        }
    }

    public async Task TransitionNewStep(Guid taskId)
    {
        var task = await GetById(taskId);
        if (task.TaskProgress.Equals(TaskProgress.InProgress))
        {
            task.TaskProgress = TaskProgress.Tested;
        }
        else if (task.TaskProgress.Equals(TaskProgress.Tested))
        {
            task.TaskProgress = TaskProgress.PendingApproval;
        }
        else if (task.TaskProgress.Equals(TaskProgress.PendingApproval))
        {
            task.TaskProgress = TaskProgress.Completed;
        }
        
        await Update(taskId, task);
    }

    public async Task DeleteTask(Guid taskId)
    {
        try
        {
            var task = await GetById(taskId, "Files");

            if (task == null)
            {
                throw new Exception("Project not found");
            }

            await Delete(taskId);
            
            foreach (var file in task.Files)
            {
                await _taskFileService.Delete(file.Id);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to delete task by {taskId}. Exception: {ex.Message}");
        }
    }

    public async Task<TaskProject> GetTaskByTitle(string title)
    {
        try
        {
            var task = await GetByPredicate(t => t.Title.Equals(title));
            if (task is null)
            {
                throw new Exception("Task not found");
            }

            return task;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get task by {title}. Exception: {ex.Message}");
        }
    }

    public async Task<List<TaskProject>> GetTasksByUserId(Guid id)
    {
        try
        {
            var user = await _userService.GetById(id);
            
            return await GetListByPredicate(t => t.Id.Equals(id));
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get task by user id {id}. Exception: {ex.Message}");
        }
    }
}