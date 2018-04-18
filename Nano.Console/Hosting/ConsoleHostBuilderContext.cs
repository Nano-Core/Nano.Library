using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Nano.Console.Hosting.Interfaces;

namespace Nano.Console.Hosting
{
    /// <summary>
    /// Context containing the common services on the <see cref="IConsoleHost" />. 
    /// Some properties may be null until set by the <see cref="IConsoleHost" />.
    /// </summary>
    public class ConsoleHostBuilderContext
    {
        /// <summary>
        /// The <see cref="IConfiguration" /> containing the merged configuration of the application and the <see cref="IConsoleHost" />.
        /// </summary>
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// The <see cref="IHostingEnvironment" /> initialized by the <see cref="IConsoleHost" />.
        /// </summary>
        public IHostingEnvironment HostingEnvironment { get; set; }
    }
}