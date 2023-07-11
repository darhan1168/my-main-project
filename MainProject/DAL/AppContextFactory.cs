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
        builder.AddJsonFile("/Users/user/Desktop/my-main-project/MainProject/UI/bin/Debug/net6.0/appsettings.json");
        var config = builder.Build();
        string connectionString = config.GetConnectionString("DefaultConnection");
    
        var optionsBuilder = new DbContextOptionsBuilder<AppContext>();
        optionsBuilder.UseSqlServer(connectionString);
    
        return optionsBuilder;
    }
}