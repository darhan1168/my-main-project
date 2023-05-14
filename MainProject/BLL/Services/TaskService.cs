using BLL.Abstraction.Interfaces;
using Core;
using DAL.Abstraction.Interfaces;
using Task = Core.Task;
using BLL.Abstraction.Interfaces;
using Core.Enums;
using Exception = System.Exception;

namespace BLL;

public class TaskService : GenericService<Task>, ITaskService
{
    private readonly IUserService _userService;
    
    public TaskService(IRepository<Task> repository, IUserService userService) :
        base(repository)
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

    public void UpdateUser(Guid taskId, string newUsername)
    {
        try
        {
            var task = GetById(taskId);
            if (task is null)
            {
                throw new Exception("Task not found");
            }

            var newUser = _userService.GetUserByUsername(newUsername);
            

            task.User = newUser;
            Update(taskId, task);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update {newUsername} in task by {taskId}. Exception: {ex.Message}");
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

    public List<Task> GetAllTasks()
    {
        try
        {
            return GetAll();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update get all tasks. Exception: {ex.Message}");
        }
    }

    public List<Task> GetTasksByTitle(string title)
    {
        try
        {
            var tasks = GetAll();
            if (tasks is null)
            {
                throw new Exception("Tasks not found");
            }

            return tasks.Where(t => t.Title.Equals(title)).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get task by {title}. Exception: {ex.Message}");
        }
    }

    public List<Task> GetTasksByUser(User user)
    {
        try
        {
            var tasks = GetAll();
            if (tasks is null)
            {
                throw new Exception("Tasks not found");
            }

            return tasks.Where(t => t.User.Equals(user)).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get task by {user.Username}. Exception: {ex.Message}");
        }
    }
}