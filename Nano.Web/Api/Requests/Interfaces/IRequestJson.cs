namespace Nano.Web.Api.Requests.Interfaces
{
    /// <summary>
    /// Base interface for json requests (POST).
    /// </summary>
    public interface IRequestJson : IRequest
    {
        /// <summary>
        /// Gets the body of the request.
        /// </summary>
        /// <returns>The body <see cref="object"/>.</returns>
        object GetBody();
    }
}