using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace Nano.Console.Hosting.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class HostingEnvironmentExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        /// <param name="contentRootPath"></param>
        /// <param name="options"></param>
        public static void InitializeConsole(this IHostingEnvironment hostingEnvironment, string contentRootPath, ConsoleHostOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (string.IsNullOrEmpty(contentRootPath))
            {
                throw new ArgumentException("A valid non-empty content root must be provided.", nameof(contentRootPath));
            }
            if (!Directory.Exists(contentRootPath))
            {
                throw new ArgumentException($"The content root '{contentRootPath}' does not exist.", nameof(contentRootPath));
            }

            hostingEnvironment.ApplicationName = options.ApplicationName;
            hostingEnvironment.ContentRootPath = contentRootPath;
            hostingEnvironment.ContentRootFileProvider = new PhysicalFileProvider(hostingEnvironment.ContentRootPath);

            var webRoot = options.ConsoleRoot;
            if (webRoot == null)
            {
                var wwwroot = Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot");
                if (Directory.Exists(wwwroot))
                {
                    hostingEnvironment.WebRootPath = wwwroot;
                }
            }
            else
            {
                hostingEnvironment.WebRootPath = Path.Combine(hostingEnvironment.ContentRootPath, webRoot);
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

            hostingEnvironment.EnvironmentName =
                options.Environment ??
                hostingEnvironment.EnvironmentName;
        }
    }
}