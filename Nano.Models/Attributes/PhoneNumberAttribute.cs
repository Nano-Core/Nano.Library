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
                var e164 = value?.ToString();
                var prefix = PhoneNumberPrefixes.Find(e164);
                var number = prefix.Value == null ? value : e164?.Replace(prefix.Value, string.Empty).Trim();

                return prefix.Value != null && number != null;
            }
            catch
            {
                return false;
            }
        }
    }
}