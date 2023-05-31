using Core;
using Core.Enums;
using Task = Core.Task;

namespace BLL.Abstraction.Interfaces;

public interface ITaskService : IGenericService<Task>
{
    void CreateTask(Task task);

    void UpdateTitle(Guid taskId, string newTitle);
    
    void UpdateDescription(Guid taskId, string newDescription);
    
    void UpdateDeadline(Guid taskId, DateTime newDeadline);
    
    void UpdateTaskPriority(Guid taskId, TaskPriority newTaskPriority);

    void UpdateTaskProgress(Guid taskId, TaskProgress newTaskProgress);

    void UpdateMaker(Guid taskId, UserRole newUserRole);

    void TransitionNewStep(Guid taskId);

    void DeleteTask(Guid taskId);
    
    List<Task> GetAllTasks();

    List<Task> GetTasksByTitle(string title);
}