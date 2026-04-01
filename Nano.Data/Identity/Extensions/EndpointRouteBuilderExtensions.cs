using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Extensions;
using Nano.Data.Abstractions.Identity.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.Data.Identity.Extensions;

internal static class EndpointRouteBuilderExtensions
{
    internal static IEndpointRouteBuilder MapEndpointLogInExternal<TFlow, TIdentity>(this IEndpointRouteBuilder builder, string providerName, string version, string root)
        where TFlow : BaseAuthFlow
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(providerName);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(root);

        var route = ActionRoutes.AUTH_LOGIN_EXTERNAL
            .Replace("{providerName}", providerName.ToLower());

        var summary = $"Signs in a user using external {providerName} authentication.";
        const string TAG = ControllerRoutes.AUTH;

        builder
            .MapPost($"{root}/{TAG.ToLower()}/{route}", LogInExternalAsync)
            .WithEndpointDefaults<LogInExternal<TFlow>, AccessToken>(summary, TAG, version, true);

        builder
            .MapPost($"{root}/{ControllerRoutes.ROUTE_VERSION_PREFIX}/{TAG.ToLower()}/{route}", LogInExternalAsync)
            .WithEndpointDefaults<LogInExternal<TFlow>, AccessToken>(summary, TAG, version, true);

        return builder;

        async Task<IResult> LogInExternalAsync(LogInExternal<TFlow> logInExternal, IAuthIdentityRepository<TIdentity> authIdentityRepository, CancellationToken cancellationToken)
        {
            var accessToken = await authIdentityRepository
                .LogInExternalAsync(providerName, logInExternal, cancellationToken);

            return Results.Ok(accessToken);
        }
    }

    internal static IEndpointRouteBuilder MapEndpointIdentitySignUpExternal<TFlow, TUser, TIdentity>(this IEndpointRouteBuilder builder, string providerName, string version, string root)
        where TFlow : BaseAuthFlow
        where TUser : class, IEntityUser<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(providerName);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(root);

        var route = ActionRoutes.IDENTITY_SIGNUP_EXTERNAL
            .Replace("{providerName}", providerName.ToLower());

        var summary = $"Registers a new user using an external {providerName} login provider.";
        var tag = $"{typeof(TUser).Name}s";

        builder
            .MapPost($"{root}/{tag.ToLower()}/{route}", SignUpExternalAsync)
            .WithEndpointDefaults<SignUpExternal<TFlow, TUser, TIdentity>, TUser>(summary, tag, version, true);

        builder
            .MapPost($"{root}/{ControllerRoutes.ROUTE_VERSION_PREFIX}/{tag.ToLower()}/{route}", SignUpExternalAsync)
            .WithEndpointDefaults<SignUpExternal<TFlow, TUser, TIdentity>, TUser>(summary, tag, version, true);

        return builder;

        async Task<IResult> SignUpExternalAsync(SignUpExternal<TFlow, TUser, TIdentity> signUpExternal, IIdentityRepository<TIdentity> identityRepository, IAuthExternalRepositoryAggregator authExternalRepositoryAggregator, CancellationToken cancellationToken)
        {
            var authenticationData = await authExternalRepositoryAggregator
                .AuthenticateAsync(providerName, signUpExternal.Flow, cancellationToken);

            var user = await identityRepository
                .SignUpExternalAsync(new SignUpExternal<TUser, TIdentity>
                {
                    User = signUpExternal.User,
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
        }
    }

    internal static IEndpointRouteBuilder MapEndpointIdentityAddExternalLogin<TFlow, TUser, TIdentity>(this IEndpointRouteBuilder builder, string providerName, string version, string root)
        where TFlow : BaseAuthFlow
        where TUser : class, IEntityUser<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(providerName);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(root);

        var route = ActionRoutes.IDENTITY_EXTERNAL_LOGINS_ADD
            .Replace("{providerName}", providerName.ToLower());

        var summary = $"Adds a {providerName} external login to a user account.";
        var tag = $"{typeof(TUser).Name}s";

        builder
            .MapPost($"{root}/{tag.ToLower()}/{route}", AddExternalLoginAsync)
            .WithEndpointDefaults<AddExternalLogin<TFlow>, ExternalLogin>(summary, tag, version, false);

        builder
            .MapPost($"{root}/{ControllerRoutes.ROUTE_VERSION_PREFIX}/{tag.ToLower()}/{route}", AddExternalLoginAsync)
            .WithEndpointDefaults<AddExternalLogin<TFlow>, ExternalLogin>(summary, tag, version, false);

        return builder;

        async Task<IResult> AddExternalLoginAsync(TIdentity id, AddExternalLogin<TFlow> signUpExternal, IIdentityRepository<TIdentity> identityRepository, IAuthExternalRepositoryAggregator authExternalRepositoryAggregator, CancellationToken cancellationToken)
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
        }
    }

    internal static IEndpointRouteBuilder MapEndpointIdentityRemoveExternalLogin<TUser, TIdentity>(this IEndpointRouteBuilder builder, string providerName, string version, string root)
        where TUser : class, IEntityUser<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(providerName);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(root);

        var route = ActionRoutes.IDENTITY_EXTERNAL_LOGINS_REMOVE
            .Replace("{providerName}", providerName.ToLower());

        var summary = $"Removes an {providerName} login from a user account.";
        var tag = $"{typeof(TUser).Name}s";

        builder
            .MapPost($"{root}/{tag.ToLower()}/{route}", RemoveExteranlLoginAsync)
            .WithEndpointDefaults(summary, tag, version, false);

        builder
            .MapPost($"{root}/{ControllerRoutes.ROUTE_VERSION_PREFIX}/{tag.ToLower()}/{route}", RemoveExteranlLoginAsync)
            .WithEndpointDefaults(summary, tag, version, false);

        builder
            .MapDelete($"{root}/{tag.ToLower()}/{route}", RemoveExteranlLoginAsync)
            .WithEndpointDefaults(summary, tag, version, false);

        builder
            .MapDelete($"{root}/{ControllerRoutes.ROUTE_VERSION_PREFIX}/{tag.ToLower()}/{route}", RemoveExteranlLoginAsync)
            .WithEndpointDefaults(summary, tag, version, false);

        return builder;

        async Task<IResult> RemoveExteranlLoginAsync(TIdentity id, IIdentityRepository<TIdentity> identityRepository, CancellationToken cancellationToken)
        {
            await identityRepository
                .RemoveExternalLoginAsync(id, providerName, cancellationToken);

            return Results.Ok();
        }
    }
}