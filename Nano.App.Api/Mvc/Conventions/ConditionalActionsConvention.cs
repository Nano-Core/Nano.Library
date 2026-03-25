using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Options;
using Nano.App.Api.Config;
using Nano.App.Api.Controllers;
using Nano.App.Api.Mvc.Extensions;
using Nano.Common.Extensions;
using Nano.Data.Abstractions.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using Nano.App.Api.Mvc.Authentication;

namespace Nano.App.Api.Mvc.Conventions;

internal sealed class ConditionalActionsConvention(AuthenticationSchemeCache authenticationSchemeProvider, IOptionsMonitor<ApiOptions> apiOptions, IOptionsMonitor<DataOptions>? dataOptions = null)
    : IControllerModelConvention
{
    private readonly IOptionsMonitor<ApiOptions> apiOptions = apiOptions ?? throw new ArgumentNullException(nameof(apiOptions));
    private readonly AuthenticationSchemeCache authenticationSchemeCache = authenticationSchemeProvider ?? throw new ArgumentNullException(nameof(authenticationSchemeProvider));
    private readonly IOptionsMonitor<DataOptions>? dataOptions = dataOptions;

    /// <summary>
    /// Applies conditional rules to controller actions based on configuration.
    /// </summary>
    /// <param name="controller">The <see cref="ControllerModel"/> to apply rules to.</param>
    public void Apply(ControllerModel controller)
    {
        ArgumentNullException.ThrowIfNull(controller);

        this.DisableAuthControllerActions(controller);
        this.DisableIdentityControllerActions(controller);
    }


    private void DisableAuthControllerActions(ControllerModel controller)
    {
        ArgumentNullException.ThrowIfNull(controller);

        var isAuthController = controller.ControllerType
            .IsTypeOf(typeof(BaseAuthController<>));

        if (!isAuthController)
        {
            return;
        }

        var disabledActions = controller.Actions
            .Where(x =>
            {
                if (nameof(BaseAuthController<>.LogInAsync).ReplaceAsync() == x.ActionName && (this.dataOptions?.CurrentValue.Identity == null || this.apiOptions.CurrentValue.Authentication.Jwt == null))
                {
                    return true;
                }

                if (nameof(BaseAuthController<>.LogInRootAsync).ReplaceAsync() == x.ActionName && this.apiOptions.CurrentValue.Authentication.Jwt?.RootLogin == null)
                {
                    return true;
                }

                if (nameof(BaseAuthController<>.LogInExternalDirectAsync).ReplaceAsync() == x.ActionName && (this.dataOptions?.CurrentValue.Identity == null || this.apiOptions.CurrentValue.Authentication.Jwt == null || this.authenticationSchemeCache.Schemes.Length == 0))
                {
                    return true;
                }

                if (nameof(BaseAuthController<>.LogInExternalDirectTransientAsync).ReplaceAsync() == x.ActionName && (this.apiOptions.CurrentValue.Authentication.Jwt == null || this.authenticationSchemeCache.Schemes.Length == 0))
                {
                    return true;
                }

                if (nameof(BaseAuthController<>.LogInExternalFacebookAsync).ReplaceAsync() == x.ActionName && (this.dataOptions?.CurrentValue.Identity == null || this.apiOptions.CurrentValue.Authentication.Jwt?.ExternalLogins.Facebook == null))
                {
                    return true;
                }

                if (nameof(BaseAuthController<>.LogInExternalFacebookTransientAsync).ReplaceAsync() == x.ActionName && this.apiOptions.CurrentValue.Authentication.Jwt?.ExternalLogins.Facebook == null)
                {
                    return true;
                }

                if (nameof(BaseAuthController<>.LogInExternalGoogleAsync).ReplaceAsync() == x.ActionName && (this.dataOptions?.CurrentValue.Identity == null || this.apiOptions.CurrentValue.Authentication.Jwt?.ExternalLogins.Google == null))
                {
                    return true;
                }

                if (nameof(BaseAuthController<>.LogInExternalGoogleTransientAsync).ReplaceAsync() == x.ActionName && this.apiOptions.CurrentValue.Authentication.Jwt?.ExternalLogins.Google == null)
                {
                    return true;
                }

                if (nameof(BaseAuthController<>.LogInExternalMicrosoftAsync).ReplaceAsync() == x.ActionName && (this.dataOptions?.CurrentValue.Identity == null || this.apiOptions.CurrentValue.Authentication.Jwt?.ExternalLogins.Microsoft == null))
                {
                    return true;
                }

                if (nameof(BaseAuthController<>.LogInExternalMicrosoftTransientAsync).ReplaceAsync() == x.ActionName && this.apiOptions.CurrentValue.Authentication.Jwt?.ExternalLogins.Microsoft == null)
                {
                    return true;
                }

                if (nameof(BaseAuthController<>.LogInRefreshAsync).ReplaceAsync() == x.ActionName && (this.dataOptions?.CurrentValue.Identity == null || this.apiOptions.CurrentValue.Authentication.Jwt == null))
                {
                    return true;
                }

                if (nameof(BaseAuthController<>.LogOutAsync).ReplaceAsync() == x.ActionName && (this.dataOptions?.CurrentValue.Identity == null || this.apiOptions.CurrentValue.Authentication.Jwt == null))
                {
                    return true;
                }

                if (nameof(BaseAuthController<>.GetExternalSchemesAsync).ReplaceAsync() == x.ActionName && this.authenticationSchemeCache.Schemes.Length == 0)
                {
                    return true;
                }

                return false;
            })
            .ToArray();

        foreach (var action in disabledActions)
        {
            controller.Actions
                .Remove(action);
        }
    }
    private void DisableIdentityControllerActions(ControllerModel controller)
    {
        ArgumentNullException.ThrowIfNull(controller);

        var isIdentityController = controller.ControllerType
            .IsTypeOf(typeof(BaseIdentityController<,,>));

        if (!isIdentityController)
        {
            return;
        }

        var disabledActions = new List<ActionModel>();

        if (this.apiOptions.CurrentValue.Authentication.Jwt?.ExternalLogins.Facebook == null)
        {
            disabledActions
                .AddRange(controller.Actions
                    .Where(x =>
                    {
                        if (nameof(BaseIdentityController<,,>.SignUpExternalFacebookAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }

                        if (nameof(BaseIdentityController<,,>.AddExternalLoginFacebookAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }

                        if (nameof(BaseIdentityController<,,>.RemoveExternalLoginFacebookAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }

                        return false;
                    }));
        }

        if (this.apiOptions.CurrentValue.Authentication.Jwt?.ExternalLogins.Google == null)
        {
            disabledActions
                .AddRange(controller.Actions
                    .Where(x =>
                    {
                        if (nameof(BaseIdentityController<,,>.SignUpExternalGoogleAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }

                        if (nameof(BaseIdentityController<,,>.AddExternalLoginGoogleAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }

                        if (nameof(BaseIdentityController<,,>.RemoveExternalLoginGoogleAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }

                        return false;
                    }));
        }

        if (this.apiOptions.CurrentValue.Authentication.Jwt?.ExternalLogins.Microsoft == null)
        {
            disabledActions
                .AddRange(controller.Actions
                    .Where(x =>
                    {
                        if (nameof(BaseIdentityController<,,>.SignUpExternalMicrosoftAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }

                        if (nameof(BaseIdentityController<,,>.AddExternalLoginMicrosoftAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }

                        if (nameof(BaseIdentityController<,,>.RemoveExternalLoginMicrosoftAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }

                        return false;
                    }));
        }

        if (this.dataOptions?.CurrentValue.Identity?.ApiKey == null)
        {
            disabledActions
                .AddRange(controller.Actions
                    .Where(x =>
                    {
                        if (nameof(BaseIdentityController<,,>.GetApiKeysAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }

                        if (nameof(BaseIdentityController<,,>.CreateApiKeyAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }

                        if (nameof(BaseIdentityController<,,>.EditApiKeyAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }

                        if (nameof(BaseIdentityController<,,>.RevokeApiKeyAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }

                        return false;
                    }));
        }

        foreach (var action in disabledActions)
        {
            controller.Actions
                .Remove(action);
        }
    }
}