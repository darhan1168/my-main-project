using Core;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace DAL;

public class AppContext : DbContext
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<TaskFile> Files { get; set; }
    public DbSet<User> Users { get; set; }

    public AppContext(DbContextOptions<AppContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { 
        optionsBuilder.UseSqlServer(@"Server=localhost;Database=MainProject;User=sa;Password=reallyStrongPwd123;TrustServerCertificate=True;");
    }
}