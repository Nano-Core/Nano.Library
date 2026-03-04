using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Options;
using Nano.App.Api.Config;
using Nano.App.Api.Controllers;
using Nano.App.Api.Mvc.Extensions;
using Nano.Common.Extensions;
using Nano.Data.Abstractions.Config;

namespace Nano.App.Api.Mvc.Conventions;

/// <summary>
/// Conditional controller actions based on application options and feature flags.
/// </summary>
public sealed class ConditionalActionsConvention : IControllerModelConvention
{
    private readonly IOptionsMonitor<ApiOptions> apiOptions;
    private readonly IOptionsMonitor<DataOptions>? dataOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConditionalActionsConvention"/> class.
    /// </summary>
    /// <param name="apiOptions">The web API options monitor.</param>
    /// <param name="dataOptions">The data options monitor (optional).</param>
    public ConditionalActionsConvention(IOptionsMonitor<ApiOptions> apiOptions, IOptionsMonitor<DataOptions>? dataOptions = null)
    {
        this.apiOptions = apiOptions ?? throw new ArgumentNullException(nameof(apiOptions));
        this.dataOptions = dataOptions;
    }

    /// <summary>
    /// Applies conditional rules to controller actions based on configuration.
    /// </summary>
    /// <param name="controller">The <see cref="ControllerModel"/> to apply rules to.</param>
    public void Apply(ControllerModel controller)
    {
        ArgumentNullException.ThrowIfNull(controller);

        this.DisableAuthControllerActions(controller);
        this.DisableAuditControllerActions(controller);
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

        if (this.apiOptions.CurrentValue.Authentication?.Jwt == null || this.apiOptions.CurrentValue.Authentication.HideAuthController)
        {
            controller.ApiExplorer.IsVisible = false;

            controller.Actions
                .Clear();
        }
        else
        {
            var disabledActions = controller.Actions
                .Where(x =>
                {
                    if (nameof(BaseAuthController<>.LogInAsync).ReplaceAsync() == x.ActionName && this.apiOptions.CurrentValue.Authentication?.Jwt == null)
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.LogInRootAsync).ReplaceAsync() == x.ActionName && this.apiOptions.CurrentValue.Authentication?.Jwt?.RootLogin == null)
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.LogInRefreshAsync).ReplaceAsync() == x.ActionName && this.apiOptions.CurrentValue.Authentication?.Jwt == null)
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.LogInExternalDirectAsync).ReplaceAsync() == x.ActionName && !(this.apiOptions.CurrentValue.Authentication?.Jwt?.ExternalLogins.IsConfigured ?? false))
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.LogInExternalDirectTransientAsync).ReplaceAsync() == x.ActionName && !(this.apiOptions.CurrentValue.Authentication?.Jwt?.ExternalLogins.IsConfigured ?? false))
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.LogInExternalGoogleAsync).ReplaceAsync() == x.ActionName && this.apiOptions.CurrentValue.Authentication?.Jwt?.ExternalLogins.Google == null)
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.LogInExternalGoogleTransientAsync).ReplaceAsync() == x.ActionName && this.apiOptions.CurrentValue.Authentication?.Jwt?.ExternalLogins.Google == null)
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.LogInExternalFacebookAsync).ReplaceAsync() == x.ActionName && this.apiOptions.CurrentValue.Authentication?.Jwt?.ExternalLogins.Facebook == null)
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.LogInExternalFacebookTransientAsync).ReplaceAsync() == x.ActionName && this.apiOptions.CurrentValue.Authentication?.Jwt?.ExternalLogins.Facebook == null)
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.LogInExternalMicrosoftAsync).ReplaceAsync() == x.ActionName && this.apiOptions.CurrentValue.Authentication?.Jwt?.ExternalLogins.Microsoft == null)
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.LogInExternalMicrosoftTransientAsync).ReplaceAsync() == x.ActionName && this.apiOptions.CurrentValue.Authentication?.Jwt?.ExternalLogins.Microsoft == null)
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.LogOutAsync).ReplaceAsync() == x.ActionName && this.apiOptions.CurrentValue.Authentication?.Jwt == null)
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.GetExternalSchemesAsync).ReplaceAsync() == x.ActionName && !(this.apiOptions.CurrentValue.Authentication?.Jwt?.ExternalLogins.IsConfigured ?? false))
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.GetExternalLoginDataGoogleAsync).ReplaceAsync() == x.ActionName && this.apiOptions.CurrentValue.Authentication?.Jwt?.ExternalLogins.Google == null)
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.GetExternalLoginDataFaceBookAsync).ReplaceAsync() == x.ActionName && this.apiOptions.CurrentValue.Authentication?.Jwt?.ExternalLogins.Facebook == null)
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.GetExternalLoginDataMicrosoftAsync).ReplaceAsync() == x.ActionName && this.apiOptions.CurrentValue.Authentication?.Jwt?.ExternalLogins.Microsoft == null)
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
    }
    private void DisableAuditControllerActions(ControllerModel controller)
    {
        ArgumentNullException.ThrowIfNull(controller);

        var isAuditController = controller.ControllerType
            .IsTypeOf(typeof(BaseAuditController<>));

        if (!isAuditController)
        {
            return;
        }

        if (this.dataOptions?.CurrentValue.UseAudit ?? false)
        {
            return;
        }

        controller.ApiExplorer.IsVisible = false;

        controller.Actions
            .Clear();
    }
    private void DisableIdentityControllerActions(ControllerModel controller)
    {
        ArgumentNullException.ThrowIfNull(controller);

        var isIdentityController = controller.ControllerType
            .IsTypeOf(typeof(BaseIdentityController<,,,>));

        if (!isIdentityController)
        {
            return;
        }

        if (this.dataOptions?.CurrentValue.Identity?.Authentication.ApiKey != null)
        {
            return;
        }

        var disabledActions2 = controller.Actions
            .Where(x =>
            {
                if (nameof(BaseIdentityController<,,,>.GetApiKeysAsync).ReplaceAsync() == x.ActionName)
                {
                    return true;
                }
                if (nameof(BaseIdentityController<,,,>.CreateApiKeyAsync).ReplaceAsync() == x.ActionName)
                {
                    return true;
                }
                if (nameof(BaseIdentityController<,,,>.EditApiKeyAsync).ReplaceAsync() == x.ActionName)
                {
                    return true;
                }
                if (nameof(BaseIdentityController<,,,>.RevokeApiKeyAsync).ReplaceAsync() == x.ActionName)
                {
                    return true;
                }

                return false;
            })
            .ToArray();

        foreach (var action in disabledActions2)
        {
            controller.Actions
                .Remove(action);
        }
    }
}