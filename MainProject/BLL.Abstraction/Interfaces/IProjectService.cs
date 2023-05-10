using Core;
using Task = Core.Task;

namespace BLL.Abstraction.Interfaces;

public interface IProjectService
{
    void CreateProject(Project project);

    void UpdateProject(Guid projectId);

    void DeleteProject(Guid projectId);

    List<Project> GetAllProjects();
    
    List<Project> GetProjectsByTitle(string title);
    
    List<Project> GetProjectsByTask(Task task);
}