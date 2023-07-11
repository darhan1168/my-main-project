using Core;

namespace BLL.Abstraction.Interfaces;

public interface IUserProjectService :  IGenericService<UserProject>
{
    void CreateUserProject(User user, Project project);
}