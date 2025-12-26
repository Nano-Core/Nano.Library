using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Nano.Data.Abstractions.Extensions;

/// <summary>
/// 
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="applicationBuilder"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IApplicationBuilder UseNanoDbMigrations(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

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