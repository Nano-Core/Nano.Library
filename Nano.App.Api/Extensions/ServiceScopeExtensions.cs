using System;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Nano.App.Api.Config;
using Nano.App.Api.Controllers;
using Nano.App.Api.Mvc.Authentication.Abstractions;
using Nano.Common;
using Nano.Common.Extensions;
using Nano.Data.Abstractions.Identity;

namespace Nano.App.Api.Extensions;

internal static class ServiceScopeExtensions
{
    internal static IServiceScope UseNanoEndpoints(this IServiceScope serviceScope, IEndpointRouteBuilder builder, ApiOptions options)
    {
        ArgumentNullException.ThrowIfNull(serviceScope);
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);

        var hasAuthController = TypeCache
            .GetAllTypes()
            .Any(x => x.IsTypeOf(typeof(BaseAuthController)));

        var hasIdentity = serviceScope
            .MapNanoIdentityEndpoints(builder, options, hasAuthController);

        if (!hasIdentity && hasAuthController)
        {
            serviceScope
                .MapNanoTransientAuthEndpoints(builder, options);
        }

        return serviceScope;
    }


    private static bool MapNanoTransientAuthEndpoints(this IServiceScope serviceScope, IEndpointRouteBuilder builder, ApiOptions options)
    {
        ArgumentNullException.ThrowIfNull(serviceScope);
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);

        var authEndpointsTask = serviceScope.ServiceProvider
            .GetService<IRegisterTransientAuthEndpointsTask>();

        if (authEndpointsTask == null)
        {
            return false;
        }

        authEndpointsTask
            .RegisterEndpoints(builder, options.Version, options.Hosting.Root);

        return true;
    }
    private static bool MapNanoIdentityEndpoints(this IServiceScope serviceScope, IEndpointRouteBuilder builder, ApiOptions options, bool hasAuth)
    {
        ArgumentNullException.ThrowIfNull(serviceScope);
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);

        var userTypes = TypeCache
            .GetAllTypes()
            .Where(x => !x.IsAbstract)
            .Select(x => x.GetGenericBaseType(typeof(BaseEntityUserController<,,>)))
            .Where(x => x != null)
            .Select(x => x!.GetGenericArguments()[0])
            .ToArray();

        var identityEndpointsTask = serviceScope.ServiceProvider
            .GetService<IRegisterDataIdentityEndpointsTask>();

        if (identityEndpointsTask == null)
        {
            return false;
        }

        identityEndpointsTask
            .RegisterEndpoints(builder, options.Version, options.Hosting.Root, userTypes, hasAuth);

        return true;
    }
}