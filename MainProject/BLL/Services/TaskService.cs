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

    public TaskService(IRepository<TaskProject> repository, IUnitOfWork unitOfWork, IUserService userService) :
        base(repository, unitOfWork)
    {
        _userService = userService;
    }
    
    public async Task CreateTask(TaskProject task)
    {
        try
        {
            await Add(task);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create {task.Title}. Exception: {ex.Message}");
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
            await Delete(taskId);
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
            throw new Exception($"Failed to get tas by {title}. Exception: {ex.Message}");
        }
    }
}