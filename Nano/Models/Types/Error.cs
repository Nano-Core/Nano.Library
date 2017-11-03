using Microsoft.AspNetCore.Http;
using Nano.Config.Providers.Hosting;

namespace Nano.Models.Types
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
        /// Summary.
        /// </summary>
        public virtual string Summary { get; set; }

        /// <summary>
        /// Description.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Errors.
        /// </summary>
        public virtual string[] Errors { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Error()
            : this(StatusCodes.Status500InternalServerError)
        {
            
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        public Error(int statusCode)
        {
            var httpStatusCode = HttpStatus.Get(statusCode);

            this.StatusCode = statusCode;
            this.Summary = httpStatusCode?.Summary;
            this.Description = httpStatusCode?.Description;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.StatusCode} {this.Summary}";
        }
    }
}