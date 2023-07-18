using Core;
using Core.Enums;
using Task = System.Threading.Tasks.Task;
using TaskProject = Core.Task;

namespace BLL.Abstraction.Interfaces;

public interface ITaskService : IGenericService<TaskProject>
{
    Task CreateTask(TaskProject task);

    Task UpdateAll(Guid taskId, TaskProject newTask);

    Task UpdateTitle(Guid taskId, string newTitle);
    
    Task UpdateDescription(Guid taskId, string newDescription);
    
    Task UpdateDeadline(Guid taskId, DateTime newDeadline);
    
    Task UpdateTaskPriority(Guid taskId, TaskPriority newTaskPriority);

    Task UpdateTaskProgress(Guid taskId, TaskProgress newTaskProgress);

    Task TransitionNewStep(Guid taskId);

    Task DeleteTask(Guid taskId);

    Task<TaskProject> GetTaskByTitle(string title);

    Task<List<TaskProject>> GetTasksByUserId(Guid id);
}