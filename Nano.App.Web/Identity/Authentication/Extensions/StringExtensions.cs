using System;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Nano.Data.Extensions;

internal static class StringExtensions
{
    internal static RsaSecurityKey CreateRsaSecurityKey(this string key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        var rsaAlgorithm = RSA.Create();
        var publicKey = Convert.FromBase64String(key);

        rsaAlgorithm
            .ImportRSAPublicKey(publicKey, out _);

        return new RsaSecurityKey(rsaAlgorithm);
    }
}