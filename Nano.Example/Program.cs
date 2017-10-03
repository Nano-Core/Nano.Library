using Microsoft.AspNetCore.Hosting;
using Nano.App;
using Nano.App.Config.Extensions;
using Nano.Data.Providers;
using Nano.Example.Data;
using Nano.Logging.Providers;

namespace Nano.Example
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
                    x.AddLogging<ConsoleLogging>();
                    x.AddDataContext<MySqlDataProvider, ExampleDbContext>();

                    // Add additional services.
                })
                .Build()
                .Run();
        }
    }
}