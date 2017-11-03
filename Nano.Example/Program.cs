using Microsoft.AspNetCore.Hosting;
using Nano.Config.Extensions;
using Nano.Config.Providers.Data;
using Nano.Config.Providers.Eventing;
using Nano.Config.Providers.Logging;
using Nano.Example.Data;

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
                    x.AddLogging<ConsoleProvider>();
                    x.AddEventing<EasyNetQProvider>();
                    x.AddDataContext<MySqlDataProvider, ExampleDbContext>();
                })
                .Build()
                .Run();
        }
    }
}