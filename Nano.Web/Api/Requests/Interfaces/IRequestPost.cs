namespace Nano.Web.Api.Requests.Interfaces
{
    /// <summary>
    /// Base interface for post requests (POST).
    /// </summary>
    public interface IRequestPost : IRequest
    {
        /// <summary>
        /// Get Body.
        /// </summary>
        object GetBody();
    }
}