using Core;
using Task = Core.Task;

namespace BLL.Abstraction.Interfaces;

public interface ITaskService
{
    void CreateTask(Task task);

    void UpdateTask(Guid taskId);

    void DeleteTask(Guid taskId);
    
    List<Task> GetAllTasks();

    List<Task> GetTasksByTitle(string title);
    
    List<Task> GetTasksByDescription(string description);
    
    List<Task> GetTasksByUser(User user);
}