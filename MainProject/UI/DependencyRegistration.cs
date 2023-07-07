using Microsoft.Extensions.DependencyInjection;
using UI.ConsoleManagers;
using UI.Interfaces;

namespace UI;

public class DependencyRegistration
{
    public static IServiceProvider Register(string connectionString)
    {
        var services = new ServiceCollection();
        services.AddScoped<AppManager>();
        services.AddScoped<UserConsoleManager>();
        services.AddScoped<TaskConsoleManager>();
        services.AddScoped<ProjectConsoleManager>();
        
        foreach (var type in typeof(IConsoleManager<>).Assembly.GetTypes()
                     .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces()
                         .Any(i=> i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsoleManager<>))))
        {
            var interfaceType = type.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsoleManager<>));
            services.AddScoped(interfaceType, type);
        }
        
        BLL.DependencyRegistration.RegisterServices(services, connectionString);

        return services.BuildServiceProvider();
    }
}