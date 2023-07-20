using Core;
using Task = System.Threading.Tasks.Task;
using TaskProject = Core.Task;

namespace BLL.Abstraction.Interfaces;

public interface IProjectService : IGenericService<Project>
{
    Task CreateProject(Project project);
    Task UpdateAll(Guid projectId, Project newProject);

    Task UpdateTitle(Guid projectId, string newTitle);
    
    Task UpdateDescription(Guid projectId, string newDescription);
    
    Task UpdateTasks(Guid projectId, List<TaskProject> newTasks);
    
    Task UpdateUsers(Guid projectId, List<UserProject> newUsers);

    Task DeleteProject(Guid projectId);

    Task UpdateCompletionRate(Guid projectId);

    // Task AddUserProject(User user, Project project);
}