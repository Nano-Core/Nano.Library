using System;
using Microsoft.AspNetCore.Identity;

namespace Nano.Security.Extensions;

/// <summary>
/// Identity Error Describer Extensions.
/// </summary>
public static class IdentityErrorDescriberExtensions
{
    /// <summary>
    /// Returns an <see cref="IdentityError"/> indicating the specified <paramref name="phoneNumber"/> is invalid.
    /// </summary>
    /// <param name="errorDescriber">The <see cref="IdentityErrorDescriber"/>.</param>
    /// <param name="phoneNumber">The phone number that is invalid.</param>
    /// <returns>An <see cref="IdentityError"/> indicating the specified <paramref name="phoneNumber"/> is invalid.</returns>
    public static IdentityError InvalidPhoneNumber(this IdentityErrorDescriber errorDescriber, string phoneNumber)
    {
        if (errorDescriber == null)
            throw new ArgumentNullException(nameof(errorDescriber));

        return new IdentityError
        {
            Code = nameof(InvalidPhoneNumber),
            Description = $"Phone number '{phoneNumber}' is invalid"
        };
    }

    /// <summary>
    /// Returns an <see cref="IdentityError"/> indicating the specified <paramref name="phoneNumber"/> is already associated with an account.
    /// </summary>
    /// <param name="errorDescriber">The <see cref="IdentityErrorDescriber"/>.</param>
    /// <param name="phoneNumber">The phone number that is already associated with an account.</param>
    /// <returns>An <see cref="IdentityError"/> indicating the specified <paramref name="phoneNumber"/> is already associated with an account.</returns>
    public static IdentityError DuplicatePhoneNumber(this IdentityErrorDescriber errorDescriber, string phoneNumber)
    {
        if (errorDescriber == null)
            throw new ArgumentNullException(nameof(errorDescriber));

        return new IdentityError
        {
            Code = nameof(DuplicatePhoneNumber),
            Description = $"Phone number '{phoneNumber}' is already taken."
        };
    }
}