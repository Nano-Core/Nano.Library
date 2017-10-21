using System;
using System.Collections.Generic;

namespace Nano.Common.Extensions
{
    /// <summary>
    /// Object Extensions.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Generates a short unique identifier for the passed an <see cref="object"/>.
        /// </summary>
        /// <param name="object">The <see cref="object"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string GetShortId(this object @object)
        {
            if (@object == null)
                throw new ArgumentNullException(nameof(@object));

            const string DEFAULT_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string DEFAULT_CHARS_NUMBERS = "VWXYZ0A1B2C3D4E5F6G7H8I9JKLMNOPQRSTU";

            var numberString = Math.Abs(@object.GetHashCode()).ToString();
            var preCharArray = DEFAULT_CHARS.ToCharArray();
            var postCharArray = DEFAULT_CHARS_NUMBERS.ToCharArray();

            int preNumber;
            int? postNumber = null;
            if (numberString.Length > int.MaxValue.ToString().Length)
            {
                var divider = numberString.Length / 2;
                preNumber = int.Parse(numberString.Substring(0, divider));
                postNumber = int.Parse(numberString.Substring(divider));
            }
            else
                preNumber = int.Parse(numberString);

            var preNumberString = ObjectExtensions.ConvertDecToBase(preNumber, preCharArray.Length, string.Empty, preCharArray);
            var postNumberString = postNumber.HasValue ? ObjectExtensions.ConvertDecToBase(postNumber.Value, postCharArray.Length, string.Empty, postCharArray) : string.Empty;
            var value = preNumberString.Length == 0 ? string.Empty : preNumberString.Length.ToString();

            return string.Concat(preNumberString, value, postNumberString);
        }

        private static string ConvertDecToBase(int number, int @base, string encodedString, IReadOnlyList<char> chars)
        {
            if (number < @base)
                return encodedString + chars[number];

            var newNumber = number / @base;
            number = number - newNumber * @base;

            encodedString = ObjectExtensions.ConvertDecToBase(newNumber, @base, encodedString, chars);
            encodedString = number < @base ? encodedString + chars[number] : encodedString;

            return encodedString;
        }
    }
}