using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nano.App.Api.Mvc.Authentication.Abstractions;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.App.Api.Mvc.Authentication.Extensions;

internal static class EndpointRouteBuilderExtensions
{
    internal static IEndpointRouteBuilder MapEndpointAuthTransient<TFlow>(this IEndpointRouteBuilder builder, string providerName, string version, string root)
        where TFlow : BaseAuthFlow
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(providerName);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(root);

        var route = ActionRoutes.AUTH_LOGIN_EXTERNAL_TRANSIENT
            .Replace("{providerName}", providerName.ToLower());

        var summary = $"Signs in a user using transient external {providerName} authentication.";
        const string TAG = ControllerRoutes.AUTH;

        builder
            .MapPost($"{root}/{TAG.ToLower()}/{route}", LogInExternalTransientAsync)
            .WithEndpointDefaults<LogInExternal<TFlow>, AccessToken>(summary, TAG, version, true);

        builder
            .MapPost($"{root}/{ControllerRoutes.ROUTE_VERSION_PREFIX}/{TAG.ToLower()}/{route}", LogInExternalTransientAsync)
            .WithEndpointDefaults<LogInExternal<TFlow>, AccessToken>(summary, TAG, version, true);

        return builder;

        async Task<IResult> LogInExternalTransientAsync(LogInExternal<TFlow> request, IAuthTransientRepository authTransientRepository, CancellationToken cancellationToken)
        {
            var accessToken = await authTransientRepository
                .LogInExternalAsync(providerName, request, cancellationToken);

            return Results.Ok(accessToken);
        }
    }
}