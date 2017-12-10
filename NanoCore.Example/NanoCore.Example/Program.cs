using Microsoft.AspNetCore.Hosting;
using Nano.App;
using Nano.App.Extensions;
using Nano.Data.Providers.MySql;
using Nano.Eventing.Providers.EasyNetQ;
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
                    x.AddDataContext<MySqlDataProvider, ExampleDbContext>(); 
                    x.AddEventing<EasyNetQProvider>(); 
                })
                .Build()
                .Run();
        }
    }
}