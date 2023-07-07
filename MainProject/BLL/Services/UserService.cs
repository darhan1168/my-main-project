using BLL.Abstraction.Interfaces;
using Core;
using DAL.Abstraction.Interfaces;
using Task = Core.Task;

namespace BLL;

public class UserService : GenericService<User>, IUserService
{
    public UserService(IRepository<User> repository,  IUnitOfWork unitOfWork) :
        base(repository, unitOfWork)
    {
    }
    
    public void Registration(User user)
    {
        try
        {
            if (user.Username is null)
            {
                throw new Exception("Username is null");
            }
            
            if (user.Email is null)
            {
                throw new Exception("Email is null");
            }
            
            user.PasswordHash = Core.Helpers.PasswordHashing.HashPassword(user.PasswordHash);
            
            if (user.PasswordHash is null)
            {
                throw new Exception("PasswordHash is null");
            }
            
            Add(user);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to add {user.Username}. Exception: {ex.Message}");
        }
    }

    public async Task<User> Authorization(string username, string password)
    {
        try
        {
            var user = await GetUserByUsername(username);

            if (user is null)
            {
                throw new Exception("User not found");
            }
            
            if (!Core.Helpers.PasswordHashing.VerifyPassword(password, user.PasswordHash))
            {
                throw new Exception("Incorrect password");
            }

            return user;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to authorization {username}. Exception: {ex.Message}");
        }
    }

    public async Task<User> GetUserByUsername(string username)
    {
        try
        {
            return await GetByPredicate(u => u.Username.Equals(username));
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get user by {username}. Exception: {ex.Message}");
        }
    }

    public async Task<User> GetUserByEmail(string email)
    {
        try
        {
            return await GetByPredicate(u => u.Email.Equals(email));
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get user by {email}. Exception: {ex.Message}");
        }
    }
}