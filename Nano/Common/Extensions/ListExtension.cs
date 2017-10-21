using System;
using System.Collections.Generic;

namespace Nano.Common.Extensions
{
    /// <summary>
    /// List Extensions.
    /// </summary>
    public static class ListExtension
    {
        /// <summary>
        /// Adds a <see cref="List{T}"/> to generic <see cref="List{KeyValuePair}"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of key.</typeparam>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="list">The <see cref="KeyValuePair{TKey,TValue}"/> to add a <see cref="KeyValuePair{TKey,TValue}"/> to.</param>
        /// <param name="key">The <see cref="string"/> key.</param>
        /// <param name="value">The <see cref="KeyValuePair{TKey,TValue}"/> value.</param>
        public static void Add<TKey, TValue>(this IList<KeyValuePair<TKey, TValue>> list, TKey key, TValue value)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var parameter = new KeyValuePair<TKey, TValue>(key, value);

            list.Add(parameter);
        }
    }
}