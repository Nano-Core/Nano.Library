using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.App.Extensions;
using Nano.App.Interfaces;

namespace Nano.App
{
    /// <summary>
    /// Base Application (abstract).
    /// </summary>
    public abstract class BaseApplication : IApplication
    {
        /// <summary>
        /// Configuration.
        /// </summary>
        protected virtual IConfiguration Configuration { get; }

        /// <summary>
        /// Constructor. 
        /// Accepting an instance of <see cref="IConfiguration"/>.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        protected BaseApplication(IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            this.Configuration = configuration;
        }

        /// <inheritdoc />
        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var provider = services.BuildServiceProvider();

            services
                .LogServices();

            return provider;
        }

        /// <inheritdoc />
        public abstract void Configure(IApplicationBuilder applicationBuilder);

        /// <inheritdoc />
        public abstract void Configure(IApplicationBuilder applicationBuilder, IHostingEnvironment hostingEnvironment, IApplicationLifetime applicationLifetime);
    }
}