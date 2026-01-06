using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nano.Data.Abstractions.Models.Identity;

namespace Nano.Data.Identity.Extensions;

internal static class UserManagerExtensions
{
    internal static Task<IdentityUserEx<TIdentity>> GetIdentityUserAsync<TIdentity>(this UserManager<IdentityUserEx<TIdentity>> userManager, TIdentity userId, CancellationToken cancellationToken = default)
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

    internal static Task<IdentityUserEx<TIdentity>> FindByPhoneNumberAsync<TIdentity>(this UserManager<IdentityUserEx<TIdentity>> userManager, string phoneNumber)
        where TIdentity : IEquatable<TIdentity>
    {
        if (userManager == null)
        {
            throw new ArgumentNullException(nameof(userManager));
        }

        return userManager.Users
            .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
    }

    internal static Task<string> GeneratePhoneNumberConfirmationTokenAsync<TUser, TIdentity>(this UserManager<IdentityUserEx<TIdentity>> userManager, TUser user)
        where TUser : IdentityUserEx<TIdentity>
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

    internal static Task<IdentityResult> ConfirmPhoneNumberAsync<TUser, TIdentity>(this UserManager<IdentityUserEx<TIdentity>> userManager, TUser user, string token)
        where TUser : IdentityUserEx<TIdentity>
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