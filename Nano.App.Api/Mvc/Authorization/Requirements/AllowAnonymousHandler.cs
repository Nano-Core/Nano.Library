using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Nano.App.Api.Mvc.Authorization.Requirements;

internal class AllowAnonymousHandler : AuthorizationHandler<AllowAnonymousRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AllowAnonymousRequirement requirement)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);

        context
            .Succeed(requirement);

        return Task.CompletedTask;
    }
}