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

                if (!this.mvcEndpointVisibility.HasIdentity)
                {
                    if (nameof(BaseAuthController<>.LogInAsync).ReplaceAsync() == x.ActionName)
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.LogInRefreshAsync).ReplaceAsync() == x.ActionName)
                    {
                        return true;
                    }

                    if (nameof(BaseAuthController<>.LogOutAsync).ReplaceAsync() == x.ActionName)
                    {
                        return true;
                    }
                }

                if (!this.mvcEndpointVisibility.HasAuthRoot)
                {
                    if (nameof(BaseAuthController<>.LogInRootAsync).ReplaceAsync() == x.ActionName)
                    {
                        return true;
                    }
                }

                if (!this.mvcEndpointVisibility.HasAuthExternalLogins)
                {
                    if (nameof(BaseAuthController<>.GetExternalSchemesAsync).ReplaceAsync() == x.ActionName)
                    {
                        return true;
                    }
                }

                if (!this.mvcEndpointVisibility.HasApiKey)
                {
                    if (nameof(BaseAuthController<>.LogInApiKeyAsync).ReplaceAsync() == x.ActionName)
                    {
                        return true;
                    }
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
                    if (!this.mvcEndpointVisibility.HasAuthExternalLogins)
                    {
                        if (nameof(BaseEntityUserController<,,>.GetExternalLoginsAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }
                    }

                    if (!this.mvcEndpointVisibility.HasJwt)
                    {
                        if (nameof(BaseEntityUserController<,,>.GetRefreshTokensAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }

                        if (nameof(BaseEntityUserController<,,>.GetActiveRefreshTokensAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }

                        if (nameof(BaseEntityUserController<,,>.DeleteRefreshTokenAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }
                    }

                    if (!this.mvcEndpointVisibility.HasApiKey)
                    {
                        if (nameof(BaseEntityUserController<,,>.GetApiKeysAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }

                        if (nameof(BaseEntityUserController<,,>.CreateApiKeyAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }

                        if (nameof(BaseEntityUserController<,,>.EditApiKeyAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }

                        if (nameof(BaseEntityUserController<,,>.RevokeApiKeyAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }

                        if (nameof(BaseEntityUserController<,,>.GetApiKeyRolesAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }

                        if (nameof(BaseEntityUserController<,,>.AssignApiKeyRoleAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }

                        if (nameof(BaseEntityUserController<,,>.RemoveApiKeyRoleAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }

                        if (nameof(BaseEntityUserController<,,>.GetApiKeyClaimsAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }

                        if (nameof(BaseEntityUserController<,,>.AssignApiKeyClaimAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }

                        if (nameof(BaseEntityUserController<,,>.ReplaceApiKeyClaimAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }

                        if (nameof(BaseEntityUserController<,,>.AssignOrReplaceApiKeyClaimAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }

                        if (nameof(BaseEntityUserController<,,>.RemoveApiKeyClaimAsync).ReplaceAsync() == x.ActionName)
                        {
                            return true;
                        }
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