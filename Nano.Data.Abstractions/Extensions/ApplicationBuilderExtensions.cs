using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Nano.Data.Abstractions.Extensions;

/// <summary>
/// Extension methods for <see cref="IApplicationBuilder"/> related to database operations.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Applies pending database migrations and seeds initial data for the application.
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/> used to configure the HTTP request pipeline.</param>
    /// <returns>The same <see cref="IApplicationBuilder"/> instance, allowing for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="applicationBuilder"/> is <c>null</c>.</exception>
    public static IApplicationBuilder UseNanoDbMigrations(this IApplicationBuilder applicationBuilder)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        using var scope = applicationBuilder.ApplicationServices
            .CreateScope();

        var dbMigrationTask = scope.ServiceProvider
            .GetService<IDbMigrationTask>();

        dbMigrationTask?
            .MigrateAndSeedAsync()
            .GetAwaiter()
            .GetResult();

        return applicationBuilder;
    }
}