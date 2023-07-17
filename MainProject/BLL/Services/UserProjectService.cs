using BLL.Abstraction.Interfaces;
using Core;
using DAL.Abstraction.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace BLL;

public class UserProjectService : GenericService<UserProject>, IUserProjectService
{
    public UserProjectService(IRepository<UserProject> repository) 
        : base(repository)
    {
    }

    public async Task CreateUserProject(User user, Project project)
    {
        var userProject = new UserProject()
        {
            User = user,
            UserId = user.Id,
            Project = project,
            ProjectId = project.Id
        };
        
        await Add(userProject);
    }
}