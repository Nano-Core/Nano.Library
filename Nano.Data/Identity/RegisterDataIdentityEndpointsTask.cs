using Microsoft.AspNetCore.Routing;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Identity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nano.Data.Identity;

internal sealed class RegisterDataIdentityEndpointsTask<TIdentity>(IEnumerable<IAuthExternalRepository> repositories) 
    : IRegisterDataIdentityEndpointsTask
    where TIdentity : IEquatable<TIdentity>
{
    private readonly IEnumerable<IAuthExternalRepository> repositories = repositories ?? throw new NullReferenceException(nameof(repositories));

    public void RegisterEndpoints(IEndpointRouteBuilder builder, string version, string root, Type[] userTypes, bool hasAuth)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(userTypes);

        if (!this.repositories.Any())
        {
            return;
        }

        foreach (var repository in repositories)
        {
            var type = repository
                .GetType();

            var repositoryInterface = type
                .GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IAuthExternalRepository<>));

            if (repositoryInterface == null)
            {
                throw new InvalidOperationException($"Type {type.Name} does not implement {nameof(IAuthExternalRepository)}<TFlow>");
            }

            var genericArgs = repositoryInterface
                .GetGenericArguments();

            foreach (var userType in userTypes)
            {
                typeof(RegisterDataIdentityEndpointsTask<TIdentity>)
                    .GetMethod(nameof(MapIdentityEndpoints), BindingFlags.NonPublic | BindingFlags.Static)?
                    .MakeGenericMethod(genericArgs[0], userType)
                    .Invoke(null, [builder, repository.ProviderName, version, root]);
            }

            if (hasAuth)
            {
                typeof(RegisterDataIdentityEndpointsTask<TIdentity>)
                    .GetMethod(nameof(MapIdentityAuthEndpoints), BindingFlags.NonPublic | BindingFlags.Static)?
                    .MakeGenericMethod(genericArgs[0])
                    .Invoke(null, [builder, repository.ProviderName, version, root]);
            }
        }
    }


    private static void MapIdentityEndpoints<TFlow, TUser>(IEndpointRouteBuilder builder, string providerName, string version, string root)
        where TFlow : BaseAuthFlow
        where TUser : class, IEntityUser<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(providerName);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(root);

        builder
            .MapEndpointIdentitySignUpExternal<TFlow, TUser, TIdentity>(providerName, version, root)
            .MapEndpointIdentityAddExternalLogin<TFlow, TUser, TIdentity>(providerName, version, root)
            .MapEndpointIdentityRemoveExternalLogin<TUser, TIdentity>(providerName, version, root);
    }
    private static void MapIdentityAuthEndpoints<TFlow>(IEndpointRouteBuilder builder, string providerName, string version, string root)
        where TFlow : BaseAuthFlow
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(providerName);
        ArgumentNullException.ThrowIfNull(root);

        builder
            .MapEndpointLogInExternal<TFlow, TIdentity>(providerName, version, root);
    }
}