using Nano.Web.Api.Requests.Attributes;
using Newtonsoft.Json;

namespace Nano.Web.Api.Requests.Interfaces
{
    /// <summary>
    /// Base interface for requests.
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// Action.
        /// </summary>
        [JsonIgnore]
        string Action { get; set; }

        /// <summary>
        /// Controller.
        /// </summary>
        [JsonIgnore]
        string Controller { get; set; }

        /// <summary>
        /// Get Route.
        /// Get the route parameters of the request, defined by properties having <see cref="RouteAttribute"/>.
        /// </summary>
        /// <returns>The route as string.</returns>
        string GetRoute();

        /// <summary>
        /// Get Querystring.
        /// Get the querystring parameters of the request, defined by properties having <see cref="QueryAttribute"/>.
        /// </summary>
        /// <returns></returns>
        string GetQuerystring();
    }
}