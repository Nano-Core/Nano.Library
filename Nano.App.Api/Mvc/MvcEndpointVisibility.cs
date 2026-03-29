using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Nano.App.Api.Mvc.Authentication.Abstractions;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.App.Api.Mvc;

internal class MvcEndpointVisibility
{
    internal bool HasJwt { get; set; }

    internal bool HasApiKey { get; set; }

    internal bool HasIdentity { get; set; }

    internal bool HasAuthRoot { get; set; }

    internal bool HasAuthExternalLogins { get; set; }

    internal static MvcEndpointVisibility Discover(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        using var scope = serviceProvider
            .CreateScope();

        var authenticationSchemeProvider = scope.ServiceProvider
            .GetRequiredService<IAuthenticationSchemeProvider>();

        var authenticationSchemes = authenticationSchemeProvider
            .GetAllSchemesAsync()
            .GetAwaiter()
            .GetResult()
            .ToArray();

        var identityRepository = scope.ServiceProvider
            .GetService<IIdentityRepository>();

        var authRootRepository = scope.ServiceProvider
            .GetService<IAuthRootRepository>();

        var authExternalRepositories = scope.ServiceProvider
            .GetRequiredService<IEnumerable<IAuthExternalRepository>>();

        return new MvcEndpointVisibility
        {
            HasJwt = authenticationSchemes.Any(y => y.Name == AuthenticationSchemes.JWT),
            HasApiKey = authenticationSchemes.Any(y => y.Name == AuthenticationSchemes.API_KEY),
            HasIdentity = identityRepository != null,
            HasAuthRoot = authRootRepository != null,
            HasAuthExternalLogins = authExternalRepositories.Any()
        };
    }
}