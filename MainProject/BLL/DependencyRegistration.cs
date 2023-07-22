using BLL.Abstraction.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BLL;

public class DependencyRegistration
{
    public static void RegisterServices(IServiceCollection services, string connectionString)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IUserProjectService, UserProjectService>();
        services.AddScoped<ITaskFileService, TaskFileService>();

        DAL.DependencyRegistration.RegisterRepositories(services, connectionString);
    }
}