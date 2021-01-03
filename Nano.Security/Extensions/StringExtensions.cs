using System;
using System.Security.Cryptography;

namespace Nano.Security.Extensions
{
    /// <summary>
    /// String Extensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Get Random Token.
        /// Generates and returns a random base64 token.
        /// </summary>
        /// <returns>A random base64 token.</returns>
        public static string GetRandomToken()
        {
            var bytes = new byte[32];
            
            using var generator = RandomNumberGenerator.Create();
           
            generator
                .GetBytes(bytes);

            var token = Convert.ToBase64String(bytes);

            return token;
        }
    }
}