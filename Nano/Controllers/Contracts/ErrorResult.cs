using System.Collections.Generic;

namespace Nano.Controllers.Contracts
{
    /// <summary>
    /// Error Result.
    /// </summary>
    public class ErrorResult
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
        public ErrorResult()
        {
            this.StatusCode = 500;
            this.Summary = this.GetSummary();
            this.Description = this.GetDescription();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        public ErrorResult(int statusCode)
        {
            this.StatusCode = statusCode;
            this.Summary = this.GetSummary();
            this.Description = this.GetDescription();
        }

        /// <summary>
        /// GetSummary
        /// </summary>
        /// <returns></returns>
        public virtual string GetSummary()
        {
            // COSMETIC: Add proper status code Summary
            return "Bad Request";
        }

        /// <summary>
        /// GetDescription
        /// </summary>
        /// <returns></returns>
        public virtual string GetDescription()
        {
            // COSMETIC: Add proper status code Description
            return "Bad Request Description";
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.StatusCode} {this.Summary}";
        }
    }
}