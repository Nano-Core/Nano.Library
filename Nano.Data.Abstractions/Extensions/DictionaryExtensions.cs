using System;
using System.Collections.Generic;

namespace Nano.Data.Abstractions.Extensions;

/// <summary>
/// Provides extension methods for working with dictionaries.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Merges two dictionaries into a new <see cref="Dictionary{TKey, TValue}"/> instance.
    /// Entries from the second dictionary will overwrite values from the first dictionary if duplicate keys are encountered.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="first">The first dictionary to merge.</param>
    /// <param name="second">The second dictionary whose values will overwrite duplicates from the first.</param>
    /// <returns>A new <see cref="Dictionary{TKey, TValue}"/> containing all entries from both dictionaries.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="first"/> or <paramref name="second"/> is <c>null</c>.</exception>
    public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);

        var result = new Dictionary<TKey, TValue>(first);

        foreach (var kvp in second)
        {
            result[kvp.Key] = kvp.Value;
        }

        return result;
    }
}