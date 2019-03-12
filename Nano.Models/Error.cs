using System;
using System.Linq;

namespace Nano.Models
{
    /// <summary>
    /// Error.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Message.
        /// </summary>
        public virtual string Summary { get; set; }

        /// <summary>
        /// Description.
        /// </summary>
        public virtual string[] Exceptions { get; set; } = new string[0];

        /// <summary>
        /// Status Code.
        /// </summary>
        public virtual int StatusCode { get; set; } = 500;

        /// <summary>
        /// Is Translated.
        /// </summary>
        public virtual bool IsTranslated { get; set; } = false;

        /// <inheritdoc />
        public override string ToString()
        {
            var exceptionsString = this.Exceptions
                .Aggregate($"Messages:{Environment.NewLine}", (current, exception) => current + exception + Environment.NewLine);

            return $"{this.StatusCode} {this.Summary}{Environment.NewLine}Messages:{Environment.NewLine}{exceptionsString}";
        }
    }
}