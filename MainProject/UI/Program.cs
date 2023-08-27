using Microsoft.Extensions.DependencyInjection;

namespace UI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.Black;
            var serviceProvider = DependencyRegistration.Register();

            using (var scope = serviceProvider.CreateScope())
            {
                var appManager = scope.ServiceProvider.GetService<AppManager>();
                appManager.Start().Wait();
            }
        }
    }
}

