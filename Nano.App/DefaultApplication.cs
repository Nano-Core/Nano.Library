using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.Data;

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

            var hostingEnvironment = applicationBuilder.ApplicationServices.GetService<IHostingEnvironment>();
            var applicationLifetime = applicationBuilder.ApplicationServices.GetService<IApplicationLifetime>();

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

            var dbContext = applicationBuilder.ApplicationServices.GetService<BaseDbContext>();

            dbContext?.EnsureCreatedAsync().Wait();
            dbContext?.EnsureMigratedAsync().Wait();
            dbContext?.EnsureSeedAsync().Wait();
            dbContext?.EnsureImportAsync().Wait();
        }
    }
}