using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Nano.App.Interfaces;

/// <summary>
/// Application.
/// </summary>
public interface IApplication : IStartup
{
    /// <summary>
    /// Configures the application.
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
    /// <param name="hostingEnvironment">The <see cref="Microsoft.AspNetCore.Hosting.IHostingEnvironment"/>.</param>
    /// <param name="applicationLifetime">The <see cref="Microsoft.AspNetCore.Hosting.IApplicationLifetime"/>.</param>
    void Configure(IApplicationBuilder applicationBuilder, IHostEnvironment hostingEnvironment, IHostApplicationLifetime applicationLifetime);
}