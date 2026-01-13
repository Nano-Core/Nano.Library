using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Nano.Eventing.Abstractions.Extensions;

/// <summary>
/// Application builder extensions to automatically register and subscribe all event handlers during app startup.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Scans for all implementations of <see cref="IEventingHandler{TEvent}"/> in the application,
    /// resolves them from the DI container, and subscribes them to the configured <see cref="IEventing"/> instance.
    /// <para>
    ///     Should be called once during application startup (e.g., in <c>Program.cs</c> or <c>Startup.Configure</c>).
    /// </para>
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The same <see cref="IApplicationBuilder"/> instance, for chaining.</returns>
    public static IApplicationBuilder UseEventHandlers(this IApplicationBuilder applicationBuilder)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        using var scope = applicationBuilder.ApplicationServices
            .CreateScope();

        var registerEventHandlersTask = scope.ServiceProvider
            .GetService<IRegisterEventHandlersTask>();

        registerEventHandlersTask?
            .RegisterEventHandlers(scope.ServiceProvider)
            .GetAwaiter()
            .GetResult();

        return applicationBuilder;
    }
}