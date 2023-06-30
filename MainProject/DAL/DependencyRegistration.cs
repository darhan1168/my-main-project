using DAL.Abstraction.Interfaces;
using Microsoft.Extensions.DependencyInjection;
namespace DAL;

public class DependencyRegistration
{
    public static void RegisterRepositories(IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
        services.AddScoped(typeof(AppContext));
        services.AddScoped(typeof(DbFactory));
    }
}