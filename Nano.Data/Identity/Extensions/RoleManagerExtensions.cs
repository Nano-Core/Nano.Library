using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Nano.Data.Identity.Extensions;

internal static class RoleManagerExtensions
{
    internal static Task<IdentityRole<TIdentity>?> GetIdentityRoleAsync<TIdentity>(this RoleManager<IdentityRole<TIdentity>> roleManager, TIdentity roleId, CancellationToken cancellationToken = default)
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(roleManager);

        var roleIdString = roleId
            .ToString();

        if (roleIdString == null)
        {
            throw new NullReferenceException(nameof(roleIdString));
        }

        return roleManager
            .FindByIdAsync(roleIdString);
    }
}