using System.Collections.Generic;
using Nano.Hosting.Entities;

namespace Nano.App.Controllers.Contracts
{
    /// <summary>
    /// Error Result.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Status Code.
        /// </summary>
        public virtual int StatusCode { get; set; }

        /// <summary>
        /// Status Summary.
        /// </summary>
        public virtual string Summary { get; set; }

        /// <summary>
        /// Status Description.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Errors.
        /// </summary>
        public virtual List<string> Errors { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Error()
            : this(500)
        {
            
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        public Error(int statusCode)
        {
            var httpStatusCode = HttpStatusCode.Get(statusCode);

            this.StatusCode = statusCode;
            this.Summary = httpStatusCode.Summary;
            this.Description = httpStatusCode.Description;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.StatusCode} {this.Summary}";
        }
    }
}