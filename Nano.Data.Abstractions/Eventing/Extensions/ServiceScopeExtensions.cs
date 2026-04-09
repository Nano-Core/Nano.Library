using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Nano.Data.Abstractions.Eventing.Extensions;

/// <summary>
/// Service scope extension methods for <see cref="IApplicationBuilder"/> to integrate Nano Library entity eventing and handlers at application startup.
/// </summary>
public static class ServiceScopeExtensions
{
    /// <summary>
    /// Initializes entity eventing and registers all entity event handlers in the application by executing the <see cref="IRegisterEntityEventingTask"/> at startup.
    /// </summary>
    /// <param name="serviceScope">The <see cref="IServiceScope"/> instance to configure.</param>
    /// <returns>The <see cref="IServiceScope"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceScope"/> is <c>null</c>.</exception>
    public static IServiceScope UseEntityEventing(this IServiceScope serviceScope)
    {
        ArgumentNullException.ThrowIfNull(serviceScope);

        var registerEntityEventingTask = serviceScope.ServiceProvider
            .GetService<IRegisterEntityEventingTask>();

        if (registerEntityEventingTask == null)
        {
            return serviceScope;
        }

        registerEntityEventingTask
            .InitializeEntityEventing()
            .GetAwaiter()
            .GetResult();

        registerEntityEventingTask
            .RegisterEntityEventHandlers(serviceScope.ServiceProvider)
            .GetAwaiter()
            .GetResult();

        return serviceScope;
    }
}