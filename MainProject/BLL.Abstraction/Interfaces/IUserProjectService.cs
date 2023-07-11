using Core;
using Task = System.Threading.Tasks.Task;

namespace BLL.Abstraction.Interfaces;

public interface IUserProjectService :  IGenericService<UserProject>
{
    Task CreateUserProject(User user, Project project);
}