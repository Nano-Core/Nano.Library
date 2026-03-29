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
        this.DisableEntityUserControllerActions(controller);
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

                if (nameof(BaseAuthController<>.LogInRefreshAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasIdentity)
                {
                    return true;
                }

                if (nameof(BaseAuthController<>.LogOutAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasIdentity)
                {
                    return true;
                }

                if (nameof(BaseAuthController<>.GetExternalSchemesAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasAuthExternalLogins)
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
    private void DisableEntityUserControllerActions(ControllerModel controller)
    {
        ArgumentNullException.ThrowIfNull(controller);

        var isIdentityController = controller.ControllerType
            .IsTypeOf(typeof(BaseEntityUserController<,,>));

        if (!isIdentityController)
        {
            return;
        }

        var disabledActions = new List<ActionModel>();

        disabledActions
            .AddRange(controller.Actions
                .Where(x =>
                {
                    if (nameof(BaseEntityUserController<,,>.GetExternalLoginsAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasAuthExternalLogins)
                    {
                        return true;
                    }

                    if (nameof(BaseEntityUserController<,,>.GetApiKeysAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasApiKey)
                    {
                        return true;
                    }

                    if (nameof(BaseEntityUserController<,,>.CreateApiKeyAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasApiKey)
                    {
                        return true;
                    }

                    if (nameof(BaseEntityUserController<,,>.EditApiKeyAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasApiKey)
                    {
                        return true;
                    }

                    if (nameof(BaseEntityUserController<,,>.RevokeApiKeyAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasApiKey)
                    {
                        return true;
                    }

                    if (nameof(BaseEntityUserController<,,>.GetRefreshTokensAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasJwt)
                    {
                        return true;
                    }

                    if (nameof(BaseEntityUserController<,,>.GetActiveRefreshTokensAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasJwt)
                    {
                        return true;
                    }

                    if (nameof(BaseEntityUserController<,,>.DeleteRefreshTokenAsync).ReplaceAsync() == x.ActionName && !this.mvcEndpointVisibility.HasJwt)
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