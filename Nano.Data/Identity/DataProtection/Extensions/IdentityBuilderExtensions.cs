using System;
using Microsoft.AspNetCore.Identity;
using Nano.Data.Identity.DataProtection.Consts;

namespace Nano.Data.Identity.DataProtection.Extensions;

internal static class IdentityBuilderExtensions
{
    internal static IdentityBuilder AddCustomTokenProvider(this IdentityBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var userType = builder.UserType;
        var totpProvider = typeof(CustomDataProtectorTokenProvider<>).MakeGenericType(userType);

        return builder
            .AddTokenProvider(CustomTokenOptions.CUSTOM_DATA_PROTECTOR_TOKEN_PROVIDER, totpProvider);
    }
}