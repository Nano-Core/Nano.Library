using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Nano.Data.Abstractions.Eventing.Extensions;

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