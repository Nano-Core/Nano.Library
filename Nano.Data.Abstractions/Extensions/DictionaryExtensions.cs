using System;
using System.Collections.Generic;
using System.Linq;

namespace Nano.Data.Abstractions.Extensions;

/// <summary>
/// Provides extension methods for working with dictionaries.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Merges two sequences into a new <see cref="IEnumerable{T}"/> of key/value pairs.
    /// Entries from the second sequence are appended after the first - duplicate keys are kept, not overwritten.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the pairs.</typeparam>
    /// <typeparam name="TValue">The type of the values in the pairs.</typeparam>
    /// <param name="first">The first sequence to merge.</param>
    /// <param name="second">The second sequence, appended after the first.</param>
    /// <returns>A new <see cref="IEnumerable{T}"/> containing all entries from both sequences.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="first"/> or <paramref name="second"/> is <c>null</c>.</exception>
    public static IEnumerable<KeyValuePair<TKey, TValue>> Merge<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> first, IEnumerable<KeyValuePair<TKey, TValue>> second)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);

        return first
            .Concat(second);
    }
}