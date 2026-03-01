using Microsoft.AspNetCore.Builder;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Nano.Data.Abstractions.Extensions;

/// <summary>
/// Service provider Extension methods for <see cref="IApplicationBuilder"/> related to database operations.
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>
    /// Applies pending database migrations and seeds initial data for the application.
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> used to configure the HTTP request pipeline.</param>
    /// <returns>The same <see cref="IServiceProvider"/> instance, allowing for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceProvider"/> is <c>null</c>.</exception>
    public static IServiceProvider UseNanoDbMigrations(this IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        using var scope = serviceProvider
            .CreateScope();

        var dbMigrationTask = scope.ServiceProvider
            .GetService<IDbMigrationTask>();

        dbMigrationTask?
            .MigrateAndSeedAsync()
            .GetAwaiter()
            .GetResult();

        return serviceProvider;
    }
}