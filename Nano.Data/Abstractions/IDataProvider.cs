using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nano.Data.Abstractions.Config;

namespace Nano.Data.Abstractions;

/// <summary>
/// Defines a data provider used to configure the application's data access layer.
/// </summary>
/// <remarks>Documentation: https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data/README.md#nanodata</remarks>
public interface IDataProvider
{
    /// <summary>
    /// Configures provider-specific services.
    /// </summary>
    /// <param name="services">The application's <see cref="IServiceCollection"/>.</param>
    /// <param name="options">The configured <see cref="DataOptions"/> for the provider.</param>
    static abstract void Configure(IServiceCollection services, DataOptions options);

    /// <summary>
    /// Configures Entity Framework Core database options for the provider.
    /// </summary>
    /// <param name="builder">The <see cref="DbContextOptionsBuilder"/> used to configure the DbContext.</param>
    /// <param name="options">The configured <see cref="DataOptions"/> for the provider.</param>
    static abstract void Configure(DbContextOptionsBuilder builder, DataOptions options);
}