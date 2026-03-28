using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nano.App.Consts;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Models;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Abstractions.Extensions;

internal static class StringExtensions
{
    internal static string ReplaceProviderName(this string route, string providerName)
    {
        ArgumentNullException.ThrowIfNull(route);
        ArgumentNullException.ThrowIfNull(providerName);

        return route
            .Replace("{providerName}", providerName);
    }
}

internal static class RouteHandlerBuilderExtensions
{
    internal static void WithEndpointDefaults<TResponse>(this RouteHandlerBuilder builder, string summary, string tag, bool allowAnonymous)
        where TResponse : notnull
    {
        if (allowAnonymous)
        {
            builder
                .AllowAnonymous();
        }
        else
        {
            builder
                .RequireAuthorization();
        }

        builder
            .Accepts<TResponse>(HttpContentType.JSON)
            .RequireCors()
            .Produces<AccessToken>()
            .ProducesProblem((int)HttpStatusCode.NotFound)
            .ProducesProblem((int)HttpStatusCode.Unauthorized)
            .ProducesProblem((int)HttpStatusCode.BadRequest)
            .ProducesProblem((int)HttpStatusCode.InternalServerError)
            .WithSummary(summary)
            .WithTags(tag)
            // BUG: .HasApiVersion(new ApiVersion(0, 0)) TEST: with versioning
            ;
    }
}

internal static class EndpointRouteBuilderExtensions
{
    internal static IEndpointRouteBuilder MapEndpointAuth<TFlow, TIdentity>(this IEndpointRouteBuilder builder, string providerName, string root)
        where TFlow : BaseAuthFlow
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(providerName);
        ArgumentNullException.ThrowIfNull(root);

        var route = $"{root}{ActionRoutes.AUTH_LOGIN_EXTERNAL}"
            .ReplaceProviderName(providerName);

        builder
            .MapPost(route, async (LogInExternal<TFlow> logInExternal, IAuthIdentityRepository<TIdentity> authIdentityRepository, CancellationToken cancellationToken) =>
            {
                var accessToken = await authIdentityRepository
                    .LogInExternalAsync(providerName, logInExternal, cancellationToken);

                return Results.Ok(accessToken);
            })
            .WithEndpointDefaults<AccessToken>($"Signs in a user using external {providerName} authentication.", ControllerRoutes.AUTH, true);

        return builder;
    }
    
    internal static IEndpointRouteBuilder MapEndpointIdentitySignUp<TFlow, TUser, TIdentity>(this IEndpointRouteBuilder builder, string providerName, string root)
        where TFlow : BaseAuthFlow
        where TUser : class, IEntityUser<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(providerName);
        ArgumentNullException.ThrowIfNull(root);

        var route = $"{root}{ActionRoutes.IDENTITY_SIGNUP_EXTERNAL}"
            .ReplaceProviderName(providerName);

        builder
            .MapPost(route, async (SignUpExternal<TFlow, TUser, TIdentity> signUpExternal, IIdentityRepository<TIdentity> identityRepository, IAuthExternalRepositoryAggregator authExternalRepositoryAggregator, CancellationToken cancellationToken) =>
            {
                var authenticationData = await authExternalRepositoryAggregator
                    .AuthenticateAsync(providerName, signUpExternal.Flow, cancellationToken);

                var user = await identityRepository
                    .SignUpExternalAsync(new SignUpExternal<TUser, TIdentity>
                    {
                        Username = authenticationData.Username,
                        EmailAddress = authenticationData.EmailAddress,
                        PhoneNumber = authenticationData.PhoneNumber,
                        ExternalProvider = new ExternalProvider
                        {
                            Name = authenticationData.ExternalToken.Name,
                            UserId = authenticationData.Id
                        }
                    }, cancellationToken);

                return Results.Created(ActionRoutes.IDENTITY_SIGNUP_EXTERNAL, user);
            })
            .WithEndpointDefaults<AccessToken>($"Registers a new user using an external {providerName} login provider.", typeof(TUser).Name, true);

        return builder;
    }
 
    internal static IEndpointRouteBuilder MapEndpointIdentityAddExternalLogin<TFlow, TUser, TIdentity>(this IEndpointRouteBuilder builder, string providerName, string root)
        where TFlow : BaseAuthFlow
        where TUser : class, IEntityUser<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(providerName);
        ArgumentNullException.ThrowIfNull(root);

        var route = $"{root}{ActionRoutes.IDENTITY_EXTERNAL_LOGINS_ADD}"
            .ReplaceProviderName(providerName);

        builder
            .MapPost(route, async (TIdentity id, AddExternalLogin<TFlow> signUpExternal, IIdentityRepository<TIdentity> identityRepository, IAuthExternalRepositoryAggregator authExternalRepositoryAggregator, CancellationToken cancellationToken) =>
            {
                var authenticationData = await authExternalRepositoryAggregator
                    .AuthenticateAsync(providerName, signUpExternal.Flow, cancellationToken);

                var userLoginInfo = await identityRepository
                    .AddExternalLoginAsync(id, new ExternalProvider
                    {
                        Name = authenticationData.ExternalToken.Name,
                        UserId = authenticationData.Id
                    }, cancellationToken);

                var externalLogin = new ExternalLogin
                {
                    Key = userLoginInfo.ProviderKey,
                    Provider = new ExternalLoginProvider
                    {
                        Name = userLoginInfo.LoginProvider,
                        DisplayName = userLoginInfo.ProviderDisplayName
                    }
                };

                return Results.Ok(externalLogin);
            })
            .WithEndpointDefaults<AccessToken>($"Adds a {providerName} external login to a user account.", typeof(TUser).Name, true);

        return builder;
    }
    
    internal static IEndpointRouteBuilder MapEndpointIdentityRemoveExternalLogin<TUser, TIdentity>(this IEndpointRouteBuilder builder, string providerName, string root)
        where TUser : class, IEntityUser<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(providerName);
        ArgumentNullException.ThrowIfNull(root);

        var route = $"{root}{ActionRoutes.IDENTITY_EXTERNAL_LOGINS_REMOVE}"
            .ReplaceProviderName(providerName);

        builder
            .MapPost(route, RemoveExteranlLogin)
            .WithEndpointDefaults<AccessToken>($"Removes an {providerName} login from a user account.", typeof(TUser).Name, true);

        builder
            .MapDelete(route, RemoveExteranlLogin)
            .WithEndpointDefaults<AccessToken>($"Removes an {providerName} login from a user account.", typeof(TUser).Name, true);

        return builder;

        async Task<IResult> RemoveExteranlLogin(TIdentity id, IIdentityRepository<TIdentity> identityRepository, CancellationToken cancellationToken)
        {
            await identityRepository
                .RemoveExternalLoginAsync(id, providerName, cancellationToken);

            return Results.Ok();
        }
    }
}