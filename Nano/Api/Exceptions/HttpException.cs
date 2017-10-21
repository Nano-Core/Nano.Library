using System;
using Nano.Api.Entities.Interfaces;

namespace Nano.Api.Exceptions
{
    /// <summary>
    /// Http Exception.
    /// </summary>
    public class HttpException : Exception
    {
        /// <summary>
        /// Response.
        /// </summary>
        public virtual IResponse Response { get; set; }

        /// <summary>
        /// Constructor, accepting a error message and a optional status.
        /// </summary>
        /// <param name="message">The error message.</param>
        public HttpException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Constructor, accepting a error message and a optional status.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public HttpException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}