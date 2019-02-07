namespace Nano.Web.Api.Requests.Interfaces
{
    /// <summary>
    /// Base interface for requests (POST).
    /// </summary>
    public interface IRequestPost : IRequest
    {
        /// <summary>
        /// Gets the body of the request.
        /// </summary>
        /// <returns>The body <see cref="object"/>.</returns>
        object GetBody();
    }
}