using System;
using System.Collections.Generic;

namespace Nano.Data.Eventing.Extensions;

internal static class DictionaryExtensions
{
    internal static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> target, IDictionary<TKey, TValue> source)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(source);

        foreach (var pair in source)
        {
            target[pair.Key] = pair.Value;
        }
    }
}