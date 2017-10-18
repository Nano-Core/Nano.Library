using System;
using System.Collections.Generic;

namespace Nano.Hosting.Extensions
{
    /// <summary>
    /// List Extensions.
    /// </summary>
    public static class ListExtension
    {
        /// <summary>
        /// Adds a <see cref="List{T}"/> to generic <see cref="List{KeyValuePair}"/>.
        /// </summary>
        /// <param name="list">The <see cref="KeyValuePair{TKey,TValue}"/> to add a <see cref="KeyValuePair{TKey,TValue}"/> to.</param>
        /// <param name="key">The <see cref="string"/> key.</param>
        /// <param name="value">The <see cref="KeyValuePair{TKey,TValue}"/> value.</param>
        public static void Add(this IList<KeyValuePair<string, string>> list, string key, string value)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var parameter = new KeyValuePair<string, string>(key, value);

            list.Add(parameter);
        }
    }
}