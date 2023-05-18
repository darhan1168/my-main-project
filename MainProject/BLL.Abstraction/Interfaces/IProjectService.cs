using Core;
using Task = Core.Task;

namespace BLL.Abstraction.Interfaces;

public interface IProjectService : IGenericService<Project>
{
    void CreateProject(Project project);

    void UpdateTitle(Guid projectId, string newTitle);
    
    void UpdateDescription(Guid projectId, string newDescription);
    
    void UpdateTasks(Guid projectId, List<Task> newTasks);

    void DeleteProject(Guid projectId);

    List<Project> GetAllProjects();
    
    List<Project> GetProjectsByTitle(string title);
    
    List<Project> GetProjectsByTask(Task task);
}