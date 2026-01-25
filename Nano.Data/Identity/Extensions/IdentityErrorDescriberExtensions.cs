using System;
using Microsoft.AspNetCore.Identity;

namespace Nano.Data.Identity.Extensions;

internal static class IdentityErrorDescriberExtensions
{
    internal static IdentityError InvalidPhoneNumber(this IdentityErrorDescriber errorDescriber, string phoneNumber)
    {
        ArgumentNullException.ThrowIfNull(errorDescriber);

        return new IdentityError
        {
            Code = nameof(InvalidPhoneNumber),
            Description = $"Phone number '{phoneNumber}' is invalid"
        };
    }

    internal static IdentityError DuplicatePhoneNumber(this IdentityErrorDescriber errorDescriber, string phoneNumber)
    {
        ArgumentNullException.ThrowIfNull(errorDescriber);

        return new IdentityError
        {
            Code = nameof(DuplicatePhoneNumber),
            Description = $"Phone number '{phoneNumber}' is already taken."
        };
    }
}