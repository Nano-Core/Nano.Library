using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Options;
using Nano.App.Web.Config;
using Nano.App.Web.Controllers;
using Nano.App.Web.Extensions;
using Nano.Data.Abstractions.Config;
using System;
using System.Linq;

namespace Nano.App.Web.Mvc.Conventions;

/// <summary>
/// 
/// </summary>
public sealed class ConditionalActionsConvention : IControllerModelConvention
{
    private readonly IOptionsMonitor<WebOptions> webOptions;
    private readonly IOptionsMonitor<DataOptions> dataOptions;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="webOptions"></param>
    /// <param name="dataOptions"></param>
    public ConditionalActionsConvention(IOptionsMonitor<WebOptions> webOptions, IOptionsMonitor<DataOptions> dataOptions)
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
        if (controller == null) 
            throw new ArgumentNullException(nameof(controller));

        if (controller.ControllerType == typeof(AuditController))
        {
            if (!(this.dataOptions?.CurrentValue.UseAudit ?? false) || !this.webOptions.CurrentValue.Hosting.ExposeAuditController)
            {
                controller.ApiExplorer.IsVisible = false;

                controller.Actions
                    .Clear();
            }
        }

        if (controller.ControllerType == typeof(DefaultAuthController))
        {
            if (!(this.webOptions.CurrentValue.Identity.Authentication.Jwt != null || this.webOptions.CurrentValue.Identity.Authentication.Jwt?.RootLogin != null) || !this.webOptions.CurrentValue.Hosting.ExposeAuthController)
            {
                controller.ApiExplorer.IsVisible = false;

                controller.Actions
                    .Clear();
            }
        }

        var disabledActions = controller.Actions
            .Where(x =>
            {
                if (controller.ControllerType != typeof(DefaultAuthController))
                {
                    return false;
                }

                if (nameof(DefaultAuthController.LogInAsync).ReplaceAsync() == x.ActionName && this.webOptions.CurrentValue.Identity?.Authentication.Jwt == null)
                {
                    return true;
                }

                if (nameof(DefaultAuthController.LogInRootAsync).ReplaceAsync() == x.ActionName && this.webOptions.CurrentValue.Identity?.Authentication.Jwt?.RootLogin == null)
                {
                    return true;
                }

                if (nameof(DefaultAuthController.LogInRefreshAsync).ReplaceAsync() == x.ActionName && this.webOptions.CurrentValue.Identity?.Authentication.Jwt == null)
                {
                    return true;
                }

                if (nameof(DefaultAuthController.LogInExternalDirectAsync).ReplaceAsync() == x.ActionName && !(this.webOptions.CurrentValue.Identity?.Authentication.Jwt?.ExternalLogins.IsConfigured ?? false))
                {
                    return true;
                }

                if (nameof(DefaultAuthController.LogInExternalDirectTransientAsync).ReplaceAsync() == x.ActionName && !(this.webOptions.CurrentValue.Identity?.Authentication.Jwt?.ExternalLogins.IsConfigured ?? false))
                {
                    return true;
                }

                if (nameof(DefaultAuthController.LogInExternalGoogleAsync).ReplaceAsync() == x.ActionName && this.webOptions.CurrentValue.Identity?.Authentication.Jwt?.ExternalLogins.Google == null)
                {
                    return true;
                }

                if (nameof(DefaultAuthController.LogInExternalGoogleTransientAsync).ReplaceAsync() == x.ActionName && this.webOptions.CurrentValue.Identity?.Authentication.Jwt?.ExternalLogins.Google == null)
                {
                    return true;
                }

                if (nameof(DefaultAuthController.LogInExternalFacebookAsync).ReplaceAsync() == x.ActionName && this.webOptions.CurrentValue.Identity?.Authentication.Jwt?.ExternalLogins.Facebook == null)
                {
                    return true;
                }

                if (nameof(DefaultAuthController.LogInExternalFacebookTransientAsync).ReplaceAsync() == x.ActionName && this.webOptions.CurrentValue.Identity?.Authentication.Jwt?.ExternalLogins.Facebook == null)
                {
                    return true;
                }

                if (nameof(DefaultAuthController.LogInExternalMicrosoftAsync).ReplaceAsync() == x.ActionName && this.webOptions.CurrentValue.Identity?.Authentication.Jwt?.ExternalLogins.Microsoft == null)
                {
                    return true;
                }

                if (nameof(DefaultAuthController.LogInExternalMicrosoftTransientAsync).ReplaceAsync() == x.ActionName && this.webOptions.CurrentValue.Identity?.Authentication.Jwt?.ExternalLogins.Microsoft == null)
                {
                    return true;
                }

                if (nameof(DefaultAuthController.LogOutAsync).ReplaceAsync() == x.ActionName && this.webOptions.CurrentValue.Identity?.Authentication.Jwt == null)
                {
                    return true;
                }

                if (nameof(DefaultAuthController.GetExternalSchemesAsync).ReplaceAsync() == x.ActionName && !(this.webOptions.CurrentValue.Identity?.Authentication.Jwt?.ExternalLogins.IsConfigured ?? false))
                {
                    return true;
                }

                if (nameof(DefaultAuthController.GetExternalLoginDataGoogleAsync).ReplaceAsync() == x.ActionName && this.webOptions.CurrentValue.Identity?.Authentication.Jwt?.ExternalLogins.Google == null)
                {
                    return true;
                }

                if (nameof(DefaultAuthController.GetExternalLoginDataFaceBookAsync).ReplaceAsync() == x.ActionName && this.webOptions.CurrentValue.Identity?.Authentication.Jwt?.ExternalLogins.Facebook == null)
                {
                    return true;
                }

                if (nameof(DefaultAuthController.GetExternalLoginDataMicrosoftAsync).ReplaceAsync() == x.ActionName && this.webOptions.CurrentValue.Identity?.Authentication.Jwt?.ExternalLogins.Microsoft == null)
                {
                    return true;
                }

                return false;
            })
            .ToArray();

        foreach (var action in disabledActions)
        {
            var exists = controller.Actions
                .Any(x => x.ActionName == action.ActionName);

            if (exists)
            {
                controller.Actions
                    .Remove(action);
            }
        }
    }
}