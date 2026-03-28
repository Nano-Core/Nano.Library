using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Nano.App.Api.Mvc.Authentication;
using Nano.App.Api.Mvc.Authentication.Abstractions;
using Nano.App.Consts;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Models;
using Nano.Data.Abstractions.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using System;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.App.Api.Extensions;

// BUG: MapEndpointAuthTransient

internal static class NanoEndpointRouteBuilderExtensions
{
    //internal static IEndpointRouteBuilder MapEndpointAuthTransient<TFlow, TIdentity>(this IEndpointRouteBuilder builder, string providerName, string root)
    //    where TFlow : BaseAuthFlow
    //    where TIdentity : IEquatable<TIdentity>
    //{
    //    ArgumentNullException.ThrowIfNull(builder);
    //    ArgumentNullException.ThrowIfNull(providerName);
    //    ArgumentNullException.ThrowIfNull(root);

    //    var route = $"{root}{ActionRoutes.AUTH_LOGIN_EXTERNAL_TRANSIENT}"
    //        .ReplaceProviderName(providerName);

    //    builder
    //        .MapPost(route, async (LogInExternal<TFlow> request, IAuthTransientRepository authTransientRepository, CancellationToken cancellationToken) =>
    //        {
    //            var accessToken = await authTransientRepository
    //                .LogInExternalAsync(providerName, request, cancellationToken);

    //            return Results.Ok(accessToken);
    //        })
    //        .WithEndpointDefaults<AccessToken>($"Signs in a user using transient external {providerName} authentication.", ControllerRoutes.AUTH, true);

    //    return builder;
    //}
}