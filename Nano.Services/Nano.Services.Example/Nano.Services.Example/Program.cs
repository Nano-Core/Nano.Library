using Microsoft.AspNetCore.Hosting;
using Nano.App;
using Nano.App.Logging.Providers;
using Nano.Services.Example.Data;

namespace Nano.Services.Example
{
    /// <summary>
    /// Program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main.
        /// </summary>
        public static void Main()
        {
            DefaultApplication
                .ConfigureApp<ExampleApplication>()
                .ConfigureServices(x =>
                {
                    // Add additional services.
                })
                .UseLogging<ConsoleLogging>()
                .UseDataContext<ExampleDbContext>()
                .Build()
                .Run();
        }
    }
}