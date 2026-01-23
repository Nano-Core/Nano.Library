using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Options;
using Nano.App.Api.Config;
using Nano.App.Api.Controllers;
using Nano.App.Api.Extensions;
using Nano.App.Api.Mvc.Extensions;
using Nano.Common.Extensions;
using Nano.Data.Abstractions.Config;

namespace Nano.App.Api.Mvc.Conventions;

/// <summary>
/// 
/// </summary>
public sealed class ConditionalActionsConvention : IControllerModelConvention
{
    private readonly IOptionsMonitor<ApiOptions> webOptions;
    private readonly IOptionsMonitor<DataOptions>? dataOptions;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="webOptions"></param>
    /// <param name="dataOptions"></param>
    public ConditionalActionsConvention(IOptionsMonitor<ApiOptions> webOptions, IOptionsMonitor<DataOptions>? dataOptions = null)
    {
        this.webOptions = webOptions ?? throw new ArgumentNullException(nameof(webOptions));
        this.dataOptions = dataOptions;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="controller"></param>
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

        if (!(this.webOptions.CurrentValue.Identity?.Authentication.Jwt != null || this.webOptions.CurrentValue.Identity?.Authentication.Jwt?.RootLogin != null) || !this.webOptions.CurrentValue.Hosting.ExposeAuthController)
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
                    if (nameof(BaseAuthController<>.LogInAsync).ReplaceAsync() == x.ActionName && this.webOptions.CurrentValue.Identity?.Authentication.Jwt == null)
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.LogInRootAsync).ReplaceAsync() == x.ActionName && this.webOptions.CurrentValue.Identity?.Authentication.Jwt?.RootLogin == null)
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.LogInRefreshAsync).ReplaceAsync() == x.ActionName && this.webOptions.CurrentValue.Identity?.Authentication.Jwt == null)
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.LogInExternalDirectAsync).ReplaceAsync() == x.ActionName && !(this.webOptions.CurrentValue.Identity?.Authentication.Jwt?.ExternalLogins.IsConfigured ?? false))
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.LogInExternalDirectTransientAsync).ReplaceAsync() == x.ActionName && !(this.webOptions.CurrentValue.Identity?.Authentication.Jwt?.ExternalLogins.IsConfigured ?? false))
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.LogInExternalGoogleAsync).ReplaceAsync() == x.ActionName && this.webOptions.CurrentValue.Identity?.Authentication.Jwt?.ExternalLogins.Google == null)
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.LogInExternalGoogleTransientAsync).ReplaceAsync() == x.ActionName && this.webOptions.CurrentValue.Identity?.Authentication.Jwt?.ExternalLogins.Google == null)
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.LogInExternalFacebookAsync).ReplaceAsync() == x.ActionName && this.webOptions.CurrentValue.Identity?.Authentication.Jwt?.ExternalLogins.Facebook == null)
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.LogInExternalFacebookTransientAsync).ReplaceAsync() == x.ActionName && this.webOptions.CurrentValue.Identity?.Authentication.Jwt?.ExternalLogins.Facebook == null)
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.LogInExternalMicrosoftAsync).ReplaceAsync() == x.ActionName && this.webOptions.CurrentValue.Identity?.Authentication.Jwt?.ExternalLogins.Microsoft == null)
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.LogInExternalMicrosoftTransientAsync).ReplaceAsync() == x.ActionName && this.webOptions.CurrentValue.Identity?.Authentication.Jwt?.ExternalLogins.Microsoft == null)
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.LogOutAsync).ReplaceAsync() == x.ActionName && this.webOptions.CurrentValue.Identity?.Authentication.Jwt == null)
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.GetExternalSchemesAsync).ReplaceAsync() == x.ActionName && !(this.webOptions.CurrentValue.Identity?.Authentication.Jwt?.ExternalLogins.IsConfigured ?? false))
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.GetExternalLoginDataGoogleAsync).ReplaceAsync() == x.ActionName && this.webOptions.CurrentValue.Identity?.Authentication.Jwt?.ExternalLogins.Google == null)
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.GetExternalLoginDataFaceBookAsync).ReplaceAsync() == x.ActionName && this.webOptions.CurrentValue.Identity?.Authentication.Jwt?.ExternalLogins.Facebook == null)
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.GetExternalLoginDataMicrosoftAsync).ReplaceAsync() == x.ActionName && this.webOptions.CurrentValue.Identity?.Authentication.Jwt?.ExternalLogins.Microsoft == null)
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
            .IsTypeOf(typeof(AuditController));

        if (!isAuditController)
        {
            return;
        }

        if ((this.dataOptions?.CurrentValue.UseAudit ?? false) && this.webOptions.CurrentValue.Hosting.ExposeAuditController)
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