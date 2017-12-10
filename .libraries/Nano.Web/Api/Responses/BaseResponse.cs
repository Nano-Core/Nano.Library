using Nano.Web.Api.Responses.Interfaces;

namespace Nano.Web.Api.Responses
{
    /// <inheritdoc />
    public abstract class BaseResponse : IResponse
    {
        /// <inheritdoc />
        public virtual string RawJson { get; set; }

        /// <inheritdoc />
        public virtual string RawQueryString { get; set; }

        /// <inheritdoc />
        public virtual string ErrorMessage { get; set; }
    }
}