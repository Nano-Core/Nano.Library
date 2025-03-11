using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Nano.Security.Extensions;

/// <summary>
/// Role Manager Extensions.
/// </summary>
public static class RoleManagerExtensions
{
    /// <summary>
    /// Get Role Async
    /// </summary>
    /// <typeparam name="TIdentity">The identity type.</typeparam>
    /// <param name="roleManager">The <see cref="RoleManager{TUser}"/>.</param>
    /// <param name="roleId">The role id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The role.</returns>
    public static Task<IdentityRole<TIdentity>> GetIdentityRoleAsync<TIdentity>(this RoleManager<IdentityRole<TIdentity>> roleManager, TIdentity roleId, CancellationToken cancellationToken = default)
        where TIdentity : IEquatable<TIdentity>
    {
        if (roleManager == null)
            throw new ArgumentNullException(nameof(roleManager));

        var roleIdString = roleId
            .ToString();

        if (roleIdString == null)
        {
            throw new ArgumentNullException(nameof(roleIdString));
        }

        return roleManager
            .FindByIdAsync(roleIdString);
    }
}