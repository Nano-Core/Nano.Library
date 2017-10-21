using Microsoft.AspNetCore.Hosting;
using Nano.App;
using Nano.Config.Extensions;
using Nano.Data.Providers;
using Nano.Eventing.Providers;
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
                .ConfigureApp<Application>()
                .ConfigureServices(x =>
                {
                    x.AddLogging<ConsoleLogging>();
                    x.AddEventing<RabbitMqEventingProvider>();
                    x.AddDataContext<MySqlDataProvider, ExampleDbContext>();
                })
                .Build()
                .Run();
        }
    }
}