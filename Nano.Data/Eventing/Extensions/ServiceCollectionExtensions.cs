using Microsoft.Extensions.DependencyInjection;
using Nano.Data.Abstractions.Eventing;
using Nano.Data.Abstractions.Eventing.Models;
using Nano.Eventing.Abstractions;
using System;

namespace Nano.Data.Eventing.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddEntityEventing<TContext, TIdentity>(this IServiceCollection services)
        where TContext : BaseDbContext<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddScoped(typeof(IEventingHandler<EntityEvent>), typeof(EntityEventingHandler<TIdentity>))
            .AddScoped<IRegisterEntityEventingHandlersTask, RegisterEntityEventingHandlersTask>();

        return services;
    }
}