using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Nano.Data.Abstractions.Eventing.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IApplicationBuilder"/> to integrate Nano Library entity event handlers at application startup.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Registers all entity event handlers in the application by executing the <see cref="IRegisterEntityEventHandlersTask"/> at startup.
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/> instance to configure.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="applicationBuilder"/> is <c>null</c>.</exception>
    public static IApplicationBuilder UseEntityEventHandlers(this IApplicationBuilder applicationBuilder)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        using var scope = applicationBuilder.ApplicationServices
            .CreateScope();

        var registerEntityEventHandlersTask = scope.ServiceProvider
            .GetService<IRegisterEntityEventHandlersTask>();

        registerEntityEventHandlersTask?
            .RegisterEntityEventHandlers(applicationBuilder.ApplicationServices)
            .GetAwaiter()
            .GetResult();

        return applicationBuilder;
    }
}