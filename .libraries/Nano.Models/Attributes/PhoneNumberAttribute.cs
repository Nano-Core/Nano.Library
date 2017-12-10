using System.ComponentModel.DataAnnotations;
using Nano.Models.Attributes.Helpers;

namespace Nano.Models.Attributes
{
    /// <summary>
    /// PhoneNumber Attribute (Validation)
    /// </summary>
    public sealed class PhoneNumberAttribute : DataTypeAttribute
    {
        /// <summary>
        /// Consrtuctor.
        /// </summary>
        public PhoneNumberAttribute()
            : base(DataType.Text)

        {
        }

        /// <inheritdoc />
        public PhoneNumberAttribute(DataType dataType)
            : base(dataType)
        {

        }

        /// <inheritdoc />
        public PhoneNumberAttribute(string customDataType)
            : base(customDataType)
        {

        }

        /// <inheritdoc />
        public override bool IsValid(object value)
        {
            try
            {
                // BUG: MODEL: IsValid. Improve PhoneNumber validation. for each prefix have the number of digits and also regex for formatting
                var e164 = value.ToString();
                var prefix = PhoneNumberPrefixes.Find(e164).Value;
                var number = e164.Replace(prefix, string.Empty).Trim();

                return prefix != null && number != null;
            }
            catch
            {
                return false;
            }
        }
    }
}