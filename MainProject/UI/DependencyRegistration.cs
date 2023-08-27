using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UI.ConsoleManagers;
using UI.Interfaces;

namespace UI;

public class DependencyRegistration
{
    public static IServiceProvider Register()
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
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        string connectionString = configuration.GetConnectionString("DefaultConnection");
        
        BLL.DependencyRegistration.RegisterServices(services, connectionString);

        return services.BuildServiceProvider();
    }
}