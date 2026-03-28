using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Nano.App.Consts;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Extensions;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data;

internal sealed class RegisterDataIdentityEndpoints<TIdentity> : IRegisterDataIdentityEndpoints
    where TIdentity : IEquatable<TIdentity>
{
    public async Task RegisterEndpoints(IEndpointRouteBuilder builder, IServiceProvider serviceProvider, string root, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(root);

        await Task.CompletedTask;

        var repositories = serviceProvider
            .GetServices<IAuthExternalRepository>();

        foreach (var repository in repositories)
        {
            var type = repository
                .GetType();

            var repositoryInterface = type
                .GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IAuthExternalRepository<>));

            if (repositoryInterface == null)
            {
                throw new InvalidOperationException($"Type {type.Name} does not implement IAuthExternalRepository<TProvider, TFlow>");
            }

            var genericArgs = type
                .GetGenericArguments();

            var method = typeof(RegisterDataIdentityEndpoints<TIdentity>)
                .GetMethod(nameof(MapEndpoint), BindingFlags.NonPublic | BindingFlags.Static)!;

            var genericMethod = method
                .MakeGenericMethod(genericArgs[0], typeof(object)); // BUG: Get TUser, typeof(object)

            genericMethod.Invoke(null, [builder, repository.ProviderName, root]);
        }
    }

    private static void MapEndpoint<TFlow, TUser>(IEndpointRouteBuilder builder, string provider, string root)
        where TFlow : BaseAuthFlow
        where TUser : class, IEntityUser<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(root);

        // BUG: Only if derived controller of BaseAuthController

        builder
            .MapEndpointAuth<TFlow, TIdentity>(provider, $"/{root}/{ControllerRoutes.AUTH}/");

        // BUG: Only if derived controller of BaseIdentityController

        var identityRoot = $"/{root}/{typeof(TUser).Name}/";

        builder
            .MapEndpointIdentitySignUp<TFlow, TUser, TIdentity>(provider, identityRoot)
            .MapEndpointIdentityAddExternalLogin<TFlow, TUser, TIdentity>(provider, identityRoot)
            .MapEndpointIdentityRemoveExternalLogin<TUser, TIdentity>(provider, identityRoot);
    }
}