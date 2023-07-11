using Core;
using Microsoft.EntityFrameworkCore;
using Task = Core.Task;

namespace DAL;

public class AppContext : DbContext
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<TaskFile> Files { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserProject> UserProjects { get; set; }

    public AppContext(DbContextOptions<AppContext> options)
        : base(options)
    {
    }
    // public AppContext(DbContextOptions<AppContext> options) : base(options)
    // {
    //     Database.EnsureCreated();
    // }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // { 
    //     optionsBuilder.UseSqlServer(@"Server=localhost;Database=Project;User=sa;Password=reallyStrongPwd123;TrustServerCertificate=True;");
    // }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserProject>()
            .HasKey(up => new { up.UserId, up.ProjectId });
    
        modelBuilder.Entity<UserProject>()
            .HasOne(up => up.User)
            .WithMany(u => u.UserProjects)
            .HasForeignKey(up => up.UserId);
    
        modelBuilder.Entity<UserProject>()
            .HasOne(up => up.Project)
            .WithMany(p => p.UserProjects)
            .HasForeignKey(up => up.ProjectId);
    }
}