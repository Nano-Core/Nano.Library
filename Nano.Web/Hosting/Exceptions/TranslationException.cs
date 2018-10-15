using System;

namespace Nano.Web.Hosting.Exceptions
{
    /// <summary>
    /// Translation Exception.
    /// </summary>
    public class TranslationException : Exception
    {
        /// <summary>
        /// Code.
        /// </summary>
        public virtual long Code { get; protected set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="message">The message.</param>
        public TranslationException(long code, string message)
            : base(message)
        {
            this.Code = code;
        }
    }
}