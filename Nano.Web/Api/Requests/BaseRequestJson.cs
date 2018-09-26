using Nano.Web.Api.Requests.Interfaces;

namespace Nano.Web.Api.Requests
{
    /// <summary>
    /// Details Request.
    /// </summary>
    public abstract class BaseRequestJson : BaseRequest, IRequestJson
    {
        /// <inheritdoc />
        public abstract object GetBody();
    }
}