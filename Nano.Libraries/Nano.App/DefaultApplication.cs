using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        public override void ConfigureServices(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            base.ConfigureServices(services);
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

            base.Configure(applicationBuilder, hostingEnvironment, applicationLifetime);
        }
    }
}