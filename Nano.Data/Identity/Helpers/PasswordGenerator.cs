using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;

namespace Nano.Security;

/// <summary>
/// Password Generator.
/// </summary>
public static class PasswordGenerator
{
    /// <summary>
    /// Generates a random password, respecting the given strength requirements.
    /// </summary>
    /// <param name="passwordOptions">The <see cref="PasswordOptions"/>.</param>
    /// <returns>The random password</returns>
    public static string Generate(PasswordOptions passwordOptions = null)
    {
        passwordOptions ??= new PasswordOptions
        {
            RequiredLength = 12,
            RequiredUniqueChars = 3,
            RequireDigit = true,
            RequireLowercase = true,
            RequireNonAlphanumeric = true,
            RequireUppercase = true
        };

        var randomCharArrays = new[]
        {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ",
            "abcdefghijkmnopqrstuvwxyz",
            "0123456789",
            "@#$%!?_-"
        };

        var random = new Random();
        var chars = new List<char>();
        var bytes = new byte[passwordOptions.RequiredLength];

        var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator
            .GetBytes(bytes);

        if (passwordOptions.RequireUppercase)
        {
            var index = bytes[chars.Count] % randomCharArrays[0].Length;

            chars
                .Insert(random.Next(0, chars.Count), randomCharArrays[0][index]);
        }

        if (passwordOptions.RequireLowercase)
        {
            var index = bytes[chars.Count] % randomCharArrays[1].Length;

            chars
                .Insert(random.Next(0, chars.Count), randomCharArrays[1][index]);
        }

        if (passwordOptions.RequireDigit)
        {
            var index = bytes[chars.Count] % randomCharArrays[2].Length;

            chars
                .Insert(random.Next(0, chars.Count), randomCharArrays[2][index]);
        }

        if (passwordOptions.RequireNonAlphanumeric)
        {
            var index = bytes[chars.Count] % randomCharArrays[3].Length;

            chars
                .Insert(random.Next(0, chars.Count), randomCharArrays[3][index]);
        }

        for (var i = chars.Count; i < passwordOptions.RequiredLength || chars.Distinct().Count() < passwordOptions.RequiredUniqueChars; i++)
        {
            var charArray = randomCharArrays[random.Next(0, randomCharArrays.Length)];
            var index = bytes[chars.Count] % charArray.Length;

            chars
                .Insert(random.Next(0, chars.Count), charArray[index]);
        }

        return new string(chars.ToArray());
    }
}