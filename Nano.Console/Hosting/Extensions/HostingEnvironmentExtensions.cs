using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace Nano.Console.Hosting.Extensions
{
    /// <summary>
    /// Hosting Environment Extensions.
    /// </summary>
    public static class HostingEnvironmentExtensions
    {
        /// <summary>
        /// Initializes the <see cref="IHostingEnvironment"/>.
        /// </summary>
        /// <param name="hostingEnvironment">The <see cref="IHostingEnvironment"/>.</param>
        /// <param name="contentRootPath">The content root path.</param>
        /// <param name="options">The <see cref="ConsoleHostOptions"/>.</param>
        public static void Initialize(this IHostingEnvironment hostingEnvironment, string contentRootPath, ConsoleHostOptions options)
        {
            if (hostingEnvironment == null)
                throw new ArgumentNullException(nameof(hostingEnvironment));

            if (string.IsNullOrEmpty(contentRootPath))
                throw new ArgumentNullException(nameof(contentRootPath));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var exists = Directory.Exists(contentRootPath);

            if (!exists)
                throw new ArgumentException($"The content root '{contentRootPath}' does not exist.", nameof(contentRootPath));

            hostingEnvironment.ContentRootPath = contentRootPath;
            hostingEnvironment.ApplicationName = options.ApplicationName;
            hostingEnvironment.ContentRootFileProvider = new PhysicalFileProvider(hostingEnvironment.ContentRootPath);

            var consoleRoot = options.ConsoleRoot;
            if (consoleRoot == null)
            {
                var wwwroot = Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot");
                if (Directory.Exists(wwwroot))
                {
                    hostingEnvironment.WebRootPath = wwwroot;
                }
            }
            else
            {
                hostingEnvironment.WebRootPath = Path.Combine(hostingEnvironment.ContentRootPath, consoleRoot);
            }

            if (!string.IsNullOrEmpty(hostingEnvironment.WebRootPath))
            {
                hostingEnvironment.WebRootPath = Path.GetFullPath(hostingEnvironment.WebRootPath);

                if (!Directory.Exists(hostingEnvironment.WebRootPath))
                {
                    Directory.CreateDirectory(hostingEnvironment.WebRootPath);
                }

                hostingEnvironment.WebRootFileProvider = new PhysicalFileProvider(hostingEnvironment.WebRootPath);
            }
            else
            {
                hostingEnvironment.WebRootFileProvider = new NullFileProvider();
            }

            hostingEnvironment.EnvironmentName = options.Environment ?? hostingEnvironment.EnvironmentName;
        }
    }
}