using Microsoft.AspNetCore.Hosting;
using Nano.App;
using Nano.App.Data.Extensions;
using Nano.App.Logging.Extensions;
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
                .GetWebHostBuilder<GlobaleApplication>()
                .ConfigureServices(x =>
                {
                    x.AddLoggingConsole();
                    x.AddDataContext<GlobaleDbContext>();
                })
                .Build()
                .Run();
        }
    }
}