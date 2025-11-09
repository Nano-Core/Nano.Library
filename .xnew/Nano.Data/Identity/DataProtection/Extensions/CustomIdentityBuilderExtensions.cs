using System;
using Microsoft.AspNetCore.Identity;
using Nano.Security.Const;

namespace Nano.Data.Identity.Extensions;

/// <summary>
/// Custom Identity Builder Extensions.
/// </summary>
public static class CustomIdentityBuilderExtensions
{
    /// <summary>
    /// Add Custom Token Provider.
    /// </summary>
    /// <param name="builder">The <see cref="IdentityBuilder"/>.</param>
    /// <returns>The <see cref="IdentityBuilder"/>.</returns>
    public static IdentityBuilder AddCustomTokenProvider(this IdentityBuilder builder)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        var userType = builder.UserType;
        var totpProvider = typeof(CustomDataProtectorTokenProvider<>).MakeGenericType(userType);

        return builder
            .AddTokenProvider(CustomTokenOptions.CUSTOM_DATA_PROTECTOR_TOKEN_PROVIDER, totpProvider);
    }
}