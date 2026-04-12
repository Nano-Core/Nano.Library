using Microsoft.Extensions.DependencyInjection;
using Nano.Data.Abstractions.Eventing;
using Nano.Data.Abstractions.Eventing.Models;
using Nano.Eventing.Abstractions;
using System;

namespace Nano.Data.Eventing.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddEntityEventing<TIdentity>(this IServiceCollection services)
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddScoped<IEventingHandler<EntityEvent>, EntityEventingHandler<TIdentity>>()
            .AddScoped<IRegisterEntityEventingTask, RegisterEntityEventingTask>();

        return services;
    }
}