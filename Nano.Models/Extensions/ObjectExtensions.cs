using System;
using System.ComponentModel;

namespace Nano.Models.Extensions
{
    /// <summary>
    /// Object Extensions.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Parse.
        /// Parses the object to type of <typeparamref name="T"></typeparamref>.
        /// </summary>
        /// <typeparam name="T">the type to parse to.</typeparam>
        /// <param name="object">the object to parse.</param>
        /// <returns>The parsed object.</returns>
        public static T Parse<T>(this object @object)
        {
            if (@object == null) 
                throw new ArgumentNullException(nameof(@object));
            
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(@object);
        }
    }
}