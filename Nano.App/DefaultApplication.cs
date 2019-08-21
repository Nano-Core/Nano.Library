using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IApplicationLifetime = Microsoft.AspNetCore.Hosting.IApplicationLifetime;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Nano.App
{
    /// <inheritdoc />
    public class DefaultApplication : BaseApplication
    {
        /// <inheritdoc />
        public DefaultApplication(IConfiguration configuration)
            : base(configuration)
        {

        }

        /// <inheritdoc />
        public override void Configure(IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var hostingEnvironment = services.GetService<IHostingEnvironment>();
            var applicationLifetime = services.GetService<IApplicationLifetime>();

            this.Configure(applicationBuilder, hostingEnvironment, applicationLifetime);
        }

        /// <inheritdoc />
        public override void Configure(IApplicationBuilder applicationBuilder, IHostingEnvironment hostingEnvironment, IApplicationLifetime applicationLifetime)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            if (hostingEnvironment == null)
                throw new ArgumentNullException(nameof(hostingEnvironment));

            if (applicationLifetime == null)
                throw new ArgumentNullException(nameof(applicationLifetime));
        }
    }
}