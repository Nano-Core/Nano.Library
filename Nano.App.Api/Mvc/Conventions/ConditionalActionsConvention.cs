using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Nano.App.Api.Controllers;
using Nano.App.Api.Mvc.Extensions;
using Nano.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nano.App.Api.Mvc.Conventions;

internal sealed class ConditionalActionsConvention(MvcEndpointVisibility mvcEndpointVisibility)
    : IControllerModelConvention
{
    private readonly MvcEndpointVisibility mvcEndpointVisibility = mvcEndpointVisibility ?? throw new ArgumentNullException(nameof(mvcEndpointVisibility));

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
                if (!this.mvcEndpointVisibility.HasJwt)
                {
                    return true;
                }

                if (nameof(BaseAuthController<>.LogInAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasIdentity)
                {
                    return true;
                }

                if (nameof(BaseAuthController<>.LogInRootAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasAuthRoot)
                {
                    return true;
                }

                if (nameof(BaseAuthController<>.LogInExternalFacebookAsync).ReplaceAsync() == x.ActionName && (!this.mvcEndpointVisibility.HasIdentity || !this.mvcEndpointVisibility.HasAuthFacebook))
                {
                    return true;
                }

                if (nameof(BaseAuthController<>.LogInExternalFacebookTransientAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasAuthFacebook)
                {
                    return true;
                }

                if (nameof(BaseAuthController<>.LogInExternalGoogleAsync).ReplaceAsync() == x.ActionName && (!this.mvcEndpointVisibility.HasIdentity || !this.mvcEndpointVisibility.HasAuthGoogle))
                {
                    return true;
                }

                if (nameof(BaseAuthController<>.LogInExternalGoogleTransientAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasAuthGoogle)
                {
                    return true;
                }

                if (nameof(BaseAuthController<>.LogInExternalMicrosoftAsync).ReplaceAsync() == x.ActionName && (!this.mvcEndpointVisibility.HasIdentity || !this.mvcEndpointVisibility.HasAuthMicrosoft))
                {
                    return true;
                }

                if (nameof(BaseAuthController<>.LogInExternalMicrosoftTransientAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasAuthMicrosoft)
                {
                    return true;
                }

                if (nameof(BaseAuthController<>.LogInRefreshAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasIdentity)
                {
                    return true;
                }

                if (nameof(BaseAuthController<>.LogOutAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasIdentity)
                {
                    return true;
                }

                if (nameof(BaseAuthController<>.GetExternalSchemesAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasAuthCustomExternalLogins)
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

        disabledActions
            .AddRange(controller.Actions
                .Where(x =>
                {
                    if (nameof(BaseIdentityController<,,>.GetExternalLoginsAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasAuthExternalLogins)
                    {
                        return true;
                    }

                    if (nameof(BaseIdentityController<,,>.SignUpExternalFacebookAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasAuthFacebook)
                    {
                        return true;
                    }

                    if (nameof(BaseIdentityController<,,>.AddExternalLoginFacebookAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasAuthFacebook)
                    {
                        return true;
                    }

                    if (nameof(BaseIdentityController<,,>.RemoveExternalLoginFacebookAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasAuthFacebook)
                    {
                        return true;
                    }

                    if (nameof(BaseIdentityController<,,>.SignUpExternalGoogleAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasAuthGoogle)
                    {
                        return true;
                    }

                    if (nameof(BaseIdentityController<,,>.AddExternalLoginGoogleAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasAuthGoogle)
                    {
                        return true;
                    }

                    if (nameof(BaseIdentityController<,,>.RemoveExternalLoginGoogleAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasAuthGoogle)
                    {
                        return true;
                    }

                    if (nameof(BaseIdentityController<,,>.SignUpExternalMicrosoftAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasAuthMicrosoft)
                    {
                        return true;
                    }

                    if (nameof(BaseIdentityController<,,>.AddExternalLoginMicrosoftAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasAuthMicrosoft)
                    {
                        return true;
                    }

                    if (nameof(BaseIdentityController<,,>.RemoveExternalLoginMicrosoftAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasAuthMicrosoft)
                    {
                        return true;
                    }

                    if (nameof(BaseIdentityController<,,>.GetApiKeysAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasApiKey)
                    {
                        return true;
                    }

                    if (nameof(BaseIdentityController<,,>.CreateApiKeyAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasApiKey)
                    {
                        return true;
                    }

                    if (nameof(BaseIdentityController<,,>.EditApiKeyAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasApiKey)
                    {
                        return true;
                    }

                    if (nameof(BaseIdentityController<,,>.RevokeApiKeyAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasApiKey)
                    {
                        return true;
                    }

                    return false;
                }));

        foreach (var action in disabledActions)
        {
            controller.Actions
                .Remove(action);
        }
    }
}