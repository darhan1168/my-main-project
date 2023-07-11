using BLL.Abstraction.Interfaces;
using Core;
using DAL.Abstraction.Interfaces;

namespace BLL;

public class UserProjectService : GenericService<UserProject>, IUserProjectService
{
    public UserProjectService(IRepository<UserProject> repository, IUnitOfWork unitOfWork) 
        : base(repository, unitOfWork)
    {
    }

    public void CreateUserProject(User user, Project project)
    {
        var userProject = new UserProject()
        {
            User = user,
            UserId = user.Id,
            Project = project,
            ProjectId = project.Id
        };
        
        Add(userProject);
    }
}