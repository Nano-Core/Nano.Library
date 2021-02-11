namespace Nano.Web.Api.Requests.Interfaces
{
    /// <summary>
    /// Base interface for put requests (PUT).
    /// </summary>
    public interface IRequestPut : IRequest
    {
        /// <summary>
        /// Get Body.
        /// </summary>
        object GetBody();
    }
}