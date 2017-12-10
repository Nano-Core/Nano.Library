using System;
using Nano.Web.Api.Responses.Interfaces;

namespace Nano.Web.Api.Exceptions
{
    /// <summary>
    /// Api Exception.
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>
        /// Response.
        /// </summary>
        public virtual IResponse Response { get; set; }

        /// <summary>
        /// Constructor, accepting a error message and a optional status.
        /// </summary>
        /// <param name="message">The error message.</param>
        public ApiException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Constructor, accepting a error message and a optional status.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ApiException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}