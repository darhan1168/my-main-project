using Core;
using Task = System.Threading.Tasks.Task;

namespace BLL.Abstraction.Interfaces;

public interface IUserService : IGenericService<User>
{
    User User { get; }
    Task Registration(User user);

    Task<User> Authorization(string username, string password);

    Task<User> GetUserByUsername(string username);

    Task<User> GetUserByEmail(string email);

    Task<bool> IsValuableUsername(string username);

    bool IsValidEmail(string email);

    Task<bool> IsValidPassword(string password);
}