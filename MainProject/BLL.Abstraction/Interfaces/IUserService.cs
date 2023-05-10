using Core;

namespace BLL.Abstraction.Interfaces;

public interface IUserService
{
    void Registration(User user);

    void Authorization(string username, string password);
    
    void ResetPassword(Guid userId);

    List<User> GetUserByUsername(string username);

    List<User> GetUserByEmail(string email);
}