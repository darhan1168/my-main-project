using Core;

namespace BLL.Abstraction.Interfaces;

public interface IUserService : IGenericService<User>
{
    void Registration(User user);

    Task<User> Authorization(string username, string password);

    Task<User> GetUserByUsername(string username);

    Task<User> GetUserByEmail(string email);
}