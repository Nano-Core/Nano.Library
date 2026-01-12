using System;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Nano.App.Api.Identity.Authentication.Extensions;

internal static class StringExtensions
{
    internal static RsaSecurityKey CreateRsaSecurityKey(this string key)
    {
        if (key == null) 
            throw new ArgumentNullException(nameof(key));
        
        var base64 = Convert.FromBase64String(key);

        var rsaAlgorithm = RSA.Create();

        rsaAlgorithm
            .ImportRSAPublicKey(base64, out _);

        return new RsaSecurityKey(rsaAlgorithm);
    }
}