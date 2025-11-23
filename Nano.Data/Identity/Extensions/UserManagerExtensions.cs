using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Nano.Security.Extensions;

internal static class UserManagerExtensions
{
    internal static Task<IdentityUser<TIdentity>> GetIdentityUserAsync<TIdentity>(this UserManager<IdentityUser<TIdentity>> userManager, TIdentity userId, CancellationToken cancellationToken = default)
        where TIdentity : IEquatable<TIdentity>
    {
        if (userManager == null)
        {
            throw new ArgumentNullException(nameof(userManager));
        }

        var userIdString = userId
            .ToString();

        if (userIdString == null)
        {
            throw new ArgumentNullException(nameof(userIdString));
        }

        return userManager
            .FindByIdAsync(userIdString);
    }

    internal static Task<IdentityUser<TIdentity>> FindByPhoneNumberAsync<TIdentity>(this UserManager<IdentityUser<TIdentity>> userManager, string phoneNumber)
        where TIdentity : IEquatable<TIdentity>
    {
        if (userManager == null)
        {
            throw new ArgumentNullException(nameof(userManager));
        }

        return userManager.Users
            .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
    }

    internal static Task<string> GeneratePhoneNumberConfirmationTokenAsync<TUser, TIdentity>(this UserManager<IdentityUser<TIdentity>> userManager, TUser user)
        where TUser : IdentityUser<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        if (userManager == null)
        {
            throw new ArgumentNullException(nameof(userManager));
        }

        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        if (user.PhoneNumber == null)
        {
            throw new ArgumentNullException(nameof(user.PhoneNumber));
        }

        return userManager
            .GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
    }

    internal static Task<IdentityResult> ConfirmPhoneNumberAsync<TUser, TIdentity>(this UserManager<IdentityUser<TIdentity>> userManager, TUser user, string token)
        where TUser : IdentityUser<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        if (userManager == null)
        {
            throw new ArgumentNullException(nameof(userManager));
        }

        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        if (token == null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        if (user.PhoneNumber == null)
        {
            throw new ArgumentNullException(nameof(user.PhoneNumber));
        }

        return userManager
            .ChangePhoneNumberAsync(user, user.PhoneNumber, token);
    }
}