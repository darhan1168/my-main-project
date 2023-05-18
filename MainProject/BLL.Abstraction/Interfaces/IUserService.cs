using Core;

namespace BLL.Abstraction.Interfaces;

public interface IUserService : IGenericService<User>
{
    void Registration(User user);

    User Authorization(string username, string password);

    User GetUserByUsername(string username);

    User GetUserByEmail(string email);
}