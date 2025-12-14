using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Nano.Eventing.Abstractions.Extensions;

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
    public static IApplicationBuilder UseEventHandlers(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        using var scope = applicationBuilder.ApplicationServices
            .CreateScope();

        var registerEventHandlersTask = scope.ServiceProvider
            .GetService<IRegisterEventHandlersTask>();

        registerEventHandlersTask?
            .RegisterEventHandlers(applicationBuilder.ApplicationServices)
            .GetAwaiter()
            .GetResult();

        return applicationBuilder;
    }
}