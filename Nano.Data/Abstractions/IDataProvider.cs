using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nano.Data.Abstractions.Config;

namespace Nano.Data.Abstractions;

// TODO: PROVIDER: Data: Postgres Provider

/// <summary>
/// Data Provider interface.
/// Defines the provider used for data context in the application.
/// </summary>
/// <remarks>Documentation: https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data</remarks>
public interface IDataProvider
{
    /// <summary>
    /// Configures the <see cref="IDataProvider"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="options">The <see cref="DataOptions"/>.</param>
    static abstract void Configure(IServiceCollection services, DataOptions options);

    /// <summary>
    /// Configures the <see cref="IDataProvider"/>.
    /// </summary>
    /// <param name="builder">The <see cref="DbContextOptionsBuilder"/>.</param>
    /// <param name="options">The <see cref="DataOptions"/>.</param>
    static abstract void Configure(DbContextOptionsBuilder builder, DataOptions options);
}