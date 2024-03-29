﻿using System.Security.Claims;
using System.Text.RegularExpressions;
using BLL.Abstraction.Interfaces;
using Core;
using DAL.Abstraction.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Task = System.Threading.Tasks.Task;

namespace BLL;

public class UserService : GenericService<User>, IUserService
{
    private readonly ISession _session;

    public User User => GetUserFromSession();
    
    public UserService(IRepository<User> repository, IHttpContextAccessor httpContextAccessor) :
        base(repository)
    {
        _session = httpContextAccessor.HttpContext.Session;
    }
    
    public async Task Registration(User user)
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
            
            await Add(user);
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
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new Exception("Invalid username or password.");
            }
            
            var user = await GetUserByUsername(username);

            if (user is null)
            {
                throw new Exception("User not found");
            }
            
            if (!Core.Helpers.PasswordHashing.VerifyPassword(password, user.PasswordHash))
            {
                throw new Exception("Incorrect password");
            }

            _session.SetString("AuthenticatedUser", JsonConvert.SerializeObject(user));

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

    public async Task<bool> IsValuableUsername(string username)
    {
        try
        {
            var user = await GetUserByUsername(username);

            if (user is null)
            {
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to check user by {username}. Exception: {ex.Message}");
        }
    }
    
    public bool IsValidEmail(string email)
    {
        string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
        Regex regex = new Regex(pattern);
        return regex.IsMatch(email);
    }
    
    public async Task<bool> IsEmailAvailable(string email)
    {
        return (await GetListByPredicate(u => u.Email == email)).ToList().Count == 0;
    }
    
    public async Task<bool> IsValidPassword(string password)
    {
        return password.Length >= 6;
    }
    
    private User GetUserFromSession()
    {
        var authenticatedUser = _session.GetString("AuthenticatedUser");

        if (!string.IsNullOrEmpty(authenticatedUser))
        {
            var user = JsonConvert.DeserializeObject<User>(authenticatedUser);
            return user;
        }

        return null; 
    }
}