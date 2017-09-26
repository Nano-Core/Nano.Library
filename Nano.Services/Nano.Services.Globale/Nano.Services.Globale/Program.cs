using Microsoft.AspNetCore.Hosting;
using Nano.App;
using Nano.App.Logging.Providers;
using Nano.Services.Globale.Data;

namespace Nano.Services.Globale
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
                .ConfigureApp<GlobaleApplication>()
                .ConfigureServices(x =>
                {
                    
                })
                .UseLogging<ConsoleLogging>()
                .UseDataContext<GlobaleDbContext>()
                .Build()
                .Run();
        }
    }
}