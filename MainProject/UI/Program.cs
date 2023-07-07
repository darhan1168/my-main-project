using Microsoft.Extensions.DependencyInjection;

namespace UI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.Black;
            var serviceProvider = DependencyRegistration.Register(@"Server=localhost;Database=MainProject;User=sa;Password=reallyStrongPwd123;TrustServerCertificate=True;");

            using (var scope = serviceProvider.CreateScope())
            {
                var appManager = scope.ServiceProvider.GetService<AppManager>();
                appManager.Start();
            }
        }
    }
}

