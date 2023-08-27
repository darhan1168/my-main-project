using DAL.Abstraction.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace DAL;

public class DependencyRegistration
{
    public static void RegisterRepositories(IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });
        
        //services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    }
}