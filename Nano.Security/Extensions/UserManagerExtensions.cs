using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nano.Models;

namespace Nano.Security.Extensions;

/// <summary>
/// User Manager Extensions.
/// </summary>
public static class UserManagerExtensions
{
    /// <summary>
    /// Find By PhoneNumbe rAsync
    /// </summary>
    /// <typeparam name="TUser">The user type.</typeparam>
    /// <typeparam name="TIdentity">The identity type.</typeparam>
    /// <param name="userManager">The <see cref="UserManager{TUser}"/>.</param>
    /// <param name="phoneNumber">the phone number.</param>
    /// <returns>The user.</returns>
    public static Task<TUser> FindByPhoneNumberAsync<TUser, TIdentity>(this UserManager<TUser> userManager, string phoneNumber)
        where TUser : IdentityUserExpanded<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        if (userManager == null)
            throw new ArgumentNullException(nameof(userManager));

        return userManager.Users
            .SingleOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
    }

    /// <summary>
    /// Generate Phone Number Confirmation Token Async.
    /// </summary>
    /// <typeparam name="TUser">The user type.</typeparam>
    /// <typeparam name="TIdentity">The identity type.</typeparam>
    /// <param name="userManager">The <see cref="UserManager{TUser}"/>.</param>
    /// <param name="user">The user.</param>
    /// <returns>The token.</returns>
    public static Task<string> GeneratePhoneNumberConfirmationTokenAsync<TUser, TIdentity>(this UserManager<TUser> userManager, TUser user)
        where TUser : IdentityUserExpanded<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        if (userManager == null)
            throw new ArgumentNullException(nameof(userManager));

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        if (user.PhoneNumber == null)
            throw new ArgumentNullException(nameof(user.PhoneNumber));

        return userManager
            .GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
    }

    /// <summary>
    /// Confirm Phone Number Async.
    /// </summary>
    /// <typeparam name="TUser">The user type.</typeparam>
    /// <typeparam name="TIdentity">The identity type.</typeparam>
    /// <param name="userManager">The <see cref="UserManager{TUser}"/>.</param>
    /// <param name="user">The user.</param>
    /// <param name="token">The confirm phone number token.</param>
    /// <returns>The <see cref="IdentityResult"/>.</returns>
    public static Task<IdentityResult> ConfirmPhoneNumberAsync<TUser, TIdentity>(this UserManager<TUser> userManager, TUser user, string token)
        where TUser : IdentityUserExpanded<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        if (userManager == null)
            throw new ArgumentNullException(nameof(userManager));

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        if (token == null)
            throw new ArgumentNullException(nameof(token));

        if (user.PhoneNumber == null)
            throw new ArgumentNullException(nameof(user.PhoneNumber));

        return userManager
            .ChangePhoneNumberAsync(user, user.PhoneNumber, token);
    }
}