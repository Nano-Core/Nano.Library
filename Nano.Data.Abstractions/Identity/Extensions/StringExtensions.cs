using Asp.Versioning;
using System;

namespace Nano.Data.Abstractions.Identity.Extensions;

/// <summary>
/// Provides extension methods for working with <see cref="string"/> values.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Converts the specified string value into a strongly typed identity value.
    /// </summary>
    /// <typeparam name="TIdentity">The target identity type to convert to. Supported types are <see cref="Guid"/>, <see cref="int"/>, <see cref="long"/>, and <see cref="string"/>.</typeparam>
    /// <param name="value">The string value to convert. This value is typically obtained from an external source such as a JWT claim.</param>
    /// <returns>The converted identity value of type <typeparamref name="TIdentity"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <typeparamref name="TIdentity"/> is not supported or when the value cannot be parsed into the specified type.</exception>
    public static TIdentity ConvertToIdentity<TIdentity>(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var target = typeof(TIdentity);

        if (target == typeof(Guid) && Guid.TryParse(value, out var guid))
        {
            return (TIdentity)(object)guid;
        }

        if (target == typeof(int) && int.TryParse(value, out var integer))
        {
            return (TIdentity)(object)integer;
        }

        if (target == typeof(long) && long.TryParse(value, out var bigInteger))
        {
            return (TIdentity)(object)bigInteger;
        }

        if (target == typeof(string))
        {
            return (TIdentity)(object)value;
        }

        throw new InvalidOperationException($"Unsupported identity type: {target.FullName}");
    }

    /// <summary>
    /// Extracts the JWT token from a raw Authorization header value (e.g. bound via <c>[FromHeader]</c>
    /// on a minimal API endpoint, where reading the full <c>HttpContext</c> isn't needed).
    /// </summary>
    /// <param name="authorizationHeader">The raw Authorization header value.</param>
    /// <returns>The JWT token string, or null if not present or invalid.</returns>
    public static string? GetJwtToken(this string? authorizationHeader)
    {
        const string PREFIX = "Bearer ";

        if (string.IsNullOrEmpty(authorizationHeader))
        {
            return null;
        }

        if (authorizationHeader.Length <= PREFIX.Length)
        {
            return null;
        }

        var value = authorizationHeader[PREFIX.Length..];

        return value == string.Empty
            ? null
            : value;
    }

    internal static ApiVersion ToApiVersion(this string version)
    {
        ArgumentNullException.ThrowIfNull(version);

        var parts = version
            .Split('.', StringSplitOptions.RemoveEmptyEntries);

        return parts.Length switch
        {
            1 => new ApiVersion(int.Parse(parts[0]), 0),
            _ => new ApiVersion(int.Parse(parts[0]), int.Parse(parts[1]))
        };
    }
}