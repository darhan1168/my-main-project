using Core;
using Task = Core.Task;

namespace BLL.Abstraction.Interfaces;

public interface IProjectService : IGenericService<Project>
{
    void CreateProject(Project project);

    void UpdateTitle(Guid projectId, string newTitle);
    
    void UpdateDescription(Guid projectId, string newDescription);
    
    void UpdateTasks(Guid projectId, List<Task> newTasks);
    
    void UpdateUsers(Guid projectId, List<UserProject> newUsers);

    void DeleteProject(Guid projectId);

    Task<double> GetCompletionRate(Guid projectId);

    void AddUserProject(User user, Project project);
}