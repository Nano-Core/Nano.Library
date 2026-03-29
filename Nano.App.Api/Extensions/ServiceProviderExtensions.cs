using System;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Nano.App.Api.Config;
using Nano.App.Api.Controllers;
using Nano.App.Api.Mvc.Authentication.Abstractions;
using Nano.Common.Helpers;
using Nano.Data.Abstractions.Identity;

namespace Nano.App.Api.Extensions;

internal static class ServiceProviderExtensions
{
    internal static IServiceProvider UseNanoEndpoints(this IServiceProvider serviceProvider, IEndpointRouteBuilder builder, ApiOptions options)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);

        var hasAuthController = TypesHelper
            .GetAllTypes()
            .Any(x => Common.Extensions.TypeExtensions.IsTypeOf(x, typeof(BaseAuthController)));

        var hasIdentity = serviceProvider
            .MapNanoIdentityEndpoints(builder, options, hasAuthController);

        if (!hasIdentity && hasAuthController)
        {
            serviceProvider
                .MapNanoTransientAuthEndpoints(builder, options);
        }

        return serviceProvider;
    }


    private static bool MapNanoTransientAuthEndpoints(this IServiceProvider serviceProvider, IEndpointRouteBuilder builder, ApiOptions options)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);

        var authEndpointsTask = serviceProvider
            .GetService<IRegisterTransientAuthEndpointsTask>();

        if (authEndpointsTask == null)
        {
            return false;
        }

        authEndpointsTask
            .RegisterEndpoints(builder, options.Version, options.Hosting.Root);

        return true;
    }
    private static bool MapNanoIdentityEndpoints(this IServiceProvider serviceProvider, IEndpointRouteBuilder builder, ApiOptions options, bool hasAuth)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);

        var userTypes = TypesHelper
            .GetAllTypes()
            .Where(x => !x.IsAbstract)
            .Select(x => x.GetGenericBaseType(typeof(BaseEntityUserController<,,>)))
            .Where(x => x != null)
            .Select(x => x!.GetGenericArguments()[0])
            .ToArray();

        var identityEndpointsTask = serviceProvider
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