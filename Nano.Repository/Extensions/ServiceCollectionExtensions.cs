using System;
using Microsoft.Extensions.DependencyInjection;
using Nano.Repository.Interfaces;

namespace Nano.Repository.Extensions;

/// <summary>
/// Service Collection Extensions.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="IRepository"/> and <see cref="IRepositorySpatial"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    internal static IServiceCollection AddRepository(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddScoped<IRepository, DefaultRepository>()
            .AddScoped<IRepositorySpatial, DefaultRepositorySpatial>();

        return services;
    }
}