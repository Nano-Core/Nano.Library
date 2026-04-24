using System;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Nano.App.Api.Mvc.Authentication.Extensions;

internal static class StringExtensions
{
    internal static RsaSecurityKey CreatePublicRsaSecurityKey(this string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        var base64 = Convert.FromBase64String(key);

        using var rsaAlgorithm = RSA.Create();

        rsaAlgorithm
            .ImportRSAPublicKey(base64, out _);

        return new RsaSecurityKey(rsaAlgorithm);
    }

    internal static RsaSecurityKey CreatePrivateRsaSecurityKey(this string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        var base64 = Convert.FromBase64String(key);

        using var rsaAlgorithm = RSA.Create();

        rsaAlgorithm
            .ImportRSAPrivateKey(base64, out _);

        return new RsaSecurityKey(rsaAlgorithm);
    }
}