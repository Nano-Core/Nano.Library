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
}