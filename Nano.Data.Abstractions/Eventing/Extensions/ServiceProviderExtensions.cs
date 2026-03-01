using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Nano.Data.Abstractions.Eventing.Extensions;

/// <summary>
/// Service provider extension methods for <see cref="IApplicationBuilder"/> to integrate Nano Library entity event handlers at application startup.
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>
    /// Registers all entity event handlers in the application by executing the <see cref="IRegisterEntityEventHandlersTask"/> at startup.
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> instance to configure.</param>
    /// <returns>The <see cref="IServiceProvider"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceProvider"/> is <c>null</c>.</exception>
    public static IServiceProvider UseEntityEventHandlers(this IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        using var scope = serviceProvider
            .CreateScope();

        var registerEntityEventHandlersTask = scope.ServiceProvider
            .GetService<IRegisterEntityEventHandlersTask>();

        registerEntityEventHandlersTask?
            .RegisterEntityEventHandlers(scope.ServiceProvider)
            .GetAwaiter()
            .GetResult();

        return serviceProvider;
    }
}