using Microsoft.AspNetCore.Hosting;
using Nano.App;
using Nano.App.Extensions;
using Nano.Data.Providers.MySql;
using Nano.Eventing.Providers.EasyNetQ;
using Nano.Logging.Providers.Serilog;
using NanoCore.Example.Data;

namespace NanoCore.Example
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
                    x.AddLogging<SerilogProvider>();
                    x.AddDataContext<MySqlProvider, ExampleDbContext>();
                    x.AddEventing<EasyNetQProvider>();
                })
                .Build()
                .Run();
        }
    }
}