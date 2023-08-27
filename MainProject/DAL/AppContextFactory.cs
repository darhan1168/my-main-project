using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DAL;

public class AppContextFactory : IDesignTimeDbContextFactory<AppContext>
{
    public AppContext CreateDbContext(string[] args)
    {
        var optionsBuilder = GetDbContextOptionsBuilder();
        return new AppContext(optionsBuilder.Options);
    }

    private DbContextOptionsBuilder<AppContext> GetDbContextOptionsBuilder()
    {
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory());
        
        string basePath = Directory.GetCurrentDirectory();
        string appSettingsPath = Path.Combine(basePath, "appsettings.json");
        
        builder.AddJsonFile(appSettingsPath);
        var config = builder.Build();
        string connectionString = config.GetConnectionString("DefaultConnection");
    
        var optionsBuilder = new DbContextOptionsBuilder<AppContext>();
        optionsBuilder.UseSqlServer(connectionString);
    
        return optionsBuilder;
    }
}