using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Routing;
using Nano.App.Api.Mvc.Authentication.Abstractions;
using Nano.App.Api.Mvc.Authentication.Extensions;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.Api.Mvc.Authentication;

internal sealed class RegisterTransientAuthEndpointsTask(IEnumerable<IAuthExternalRepository> repositories) : IRegisterTransientAuthEndpointsTask
{
    private readonly IEnumerable<IAuthExternalRepository> repositories = repositories ?? throw new NullReferenceException(nameof(repositories));

    public void RegisterEndpoints(IEndpointRouteBuilder builder, string version, string root)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(root);

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

            typeof(RegisterTransientAuthEndpointsTask)
                .GetMethod(nameof(MapAuthEndpoints), BindingFlags.NonPublic | BindingFlags.Static)?
                .MakeGenericMethod(genericArgs[0])
                .Invoke(null, [builder, repository.ProviderName, version, root]);
        }
    }


    private static void MapAuthEndpoints<TFlow>(IEndpointRouteBuilder builder, string providerName, string version, string root)
        where TFlow : BaseAuthFlow
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(providerName);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(root);

        builder
            .MapEndpointAuthTransient<TFlow>(providerName, version, root);
    }
}