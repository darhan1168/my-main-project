using BLL.Abstraction.Interfaces;
using Core;
using DAL.Abstraction.Interfaces;
using Task = Core.Task;
using BLL.Abstraction.Interfaces;
using Core.Enums;
using DAL;
using Exception = System.Exception;

namespace BLL;

public class TaskService : GenericService<Task>, ITaskService
{
    private readonly IUserService _userService;

    public TaskService(IRepository<Task> repository, IUnitOfWork unitOfWork, IUserService userService) :
        base(repository, unitOfWork)
    {
        _userService = userService;
    }
    
    public void CreateTask(Task task)
    {
        try
        {
            Add(task);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create {task.Title}. Exception: {ex.Message}");
        }
    }

    public void UpdateTitle(Guid taskId, string newTitle)
    {
        try
        {
            var task = GetById(taskId);
            if (task is null)
            {
                throw new Exception("Task not found");
            }

            task.Title = newTitle;
            Update(taskId, task);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update {newTitle} in task by {taskId}. Exception: {ex.Message}");
        }
    }

    public void UpdateDescription(Guid taskId, string newDescription)
    {
        try
        {
            var task = GetById(taskId);
            if (task is null)
            {
                throw new Exception("Task not found");
            }

            task.Description = newDescription;
            Update(taskId, task);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update {newDescription} in task by {taskId}. Exception: {ex.Message}");
        }
    }

    public void UpdateDeadline(Guid taskId, DateTime newDeadline)
    {
        try
        {
            var task = GetById(taskId);
            if (task is null)
            {
                throw new Exception("Task not found");
            }

            task.Deadline = newDeadline;
            Update(taskId, task);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update {newDeadline.ToShortDateString()} in task by {taskId}. Exception: {ex.Message}");
        }
    }

    public void UpdateTaskPriority(Guid taskId, TaskPriority newTaskPriority)
    {
        try
        {
            var task = GetById(taskId);
            if (task is null)
            {
                throw new Exception("Task not found");
            }

            task.TaskPriority = newTaskPriority;
            Update(taskId, task);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update {newTaskPriority} in task by {taskId}. Exception: {ex.Message}");
        }
    }

    public void UpdateTaskProgress(Guid taskId, TaskProgress newTaskProgress)
    {
        try
        {
            var task = GetById(taskId);
            if (task is null)
            {
                throw new Exception("Task not found");
            }

            task.TaskProgress = newTaskProgress;
            Update(taskId, task);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update {newTaskProgress} in task by {taskId}. Exception: {ex.Message}");
        }
    }

    public void TransitionNewStep(Guid taskId)
    {
        var task = GetById(taskId);
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
        
        Update(taskId, task);
    }

    public void CheckTask()
    {
        throw new NotImplementedException();
    }

    public void DeleteTask(Guid taskId)
    {
        try
        {
            Delete(taskId);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to delete task by {taskId}. Exception: {ex.Message}");
        }
    }

    public async Task<Task> GetTaskByTitle(string title)
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