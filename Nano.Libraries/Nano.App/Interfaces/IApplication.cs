using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Nano.App.Interfaces
{
    /// <summary>
    /// Application.
    /// </summary>
    public interface IApplication
    {
        /// <summary>
        /// Configure Services.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        void ConfigureServices(IServiceCollection services);

        /// <summary>
        /// Configures the application.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="hostingEnvironment">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="applicationLifetime">The <see cref="IApplicationBuilder"/>.</param>
        void Configure(IApplicationBuilder applicationBuilder, IHostingEnvironment hostingEnvironment, IApplicationLifetime applicationLifetime);
    }
}