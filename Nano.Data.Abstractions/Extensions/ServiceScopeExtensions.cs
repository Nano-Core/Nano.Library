using Microsoft.AspNetCore.Builder;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Nano.Data.Abstractions.Extensions;

/// <summary>
/// Service scope Extension methods for <see cref="IApplicationBuilder"/> related to database operations.
/// </summary>
public static class ServiceScopeExtensions
{
    /// <summary>
    /// Applies pending database migrations and seeds initial data for the application.
    /// </summary>
    /// <param name="serviceScope">The <see cref="IServiceScope"/> used to configure the HTTP request pipeline.</param>
    /// <returns>The same <see cref="IServiceScope"/> instance, allowing for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceScope"/> is <c>null</c>.</exception>
    public static IServiceScope UseNanoDbMigrations(this IServiceScope serviceScope)
    {
        ArgumentNullException.ThrowIfNull(serviceScope);

        var dbMigrationTask = serviceScope.ServiceProvider
            .GetService<IDbMigrationTask>();

        dbMigrationTask?
            .MigrateAndSeedAsync()
            .GetAwaiter()
            .GetResult();

        return serviceScope;
    }
}