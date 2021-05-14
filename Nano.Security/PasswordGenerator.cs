using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Nano.Security
{
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

            var randomChars = new[] { "ABCDEFGHJKLMNOPQRSTUVWXYZ", "abcdefghijkmnopqrstuvwxyz", "0123456789", "@#$%!?_-" };
            var random = new Random();
            var chars = new List<char>();

            if (passwordOptions.RequireUppercase)
            {
                chars
                    .Insert(random.Next(0, chars.Count), randomChars[0][random.Next(0, randomChars[0].Length)]);
            }

            if (passwordOptions.RequireLowercase)
            {
                chars
                    .Insert(random.Next(0, chars.Count), randomChars[1][random.Next(0, randomChars[1].Length)]);
            }

            if (passwordOptions.RequireDigit)
            {
                chars
                    .Insert(random.Next(0, chars.Count), randomChars[2][random.Next(0, randomChars[2].Length)]);
            }

            if (passwordOptions.RequireNonAlphanumeric)
            {
                chars
                    .Insert(random.Next(0, chars.Count), randomChars[3][random.Next(0, randomChars[3].Length)]);
            }

            for (var i = chars.Count; i < passwordOptions.RequiredLength || chars.Distinct().Count() < passwordOptions.RequiredUniqueChars; i++)
            {
                var randomChar = randomChars[random.Next(0, randomChars.Length)];

                chars
                    .Insert(random.Next(0, chars.Count), randomChar[random.Next(0, randomChar.Length)]);
            }

            return new string(chars.ToArray());
        }
    }
}