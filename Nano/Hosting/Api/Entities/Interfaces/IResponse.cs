namespace Nano.Hosting.Api.Entities.Interfaces
{
    /// <summary>
    /// Base interface for responses.
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// Raw json of the response.
        /// </summary>
        string RawJson { get; set; }

        /// <summary>
        /// Raw querystring of the <see cref="IRequest"/>.
        /// </summary>
        string RawQueryString { get; set; }

        /// <summary>
        /// Error Message.
        /// </summary>
        string ErrorMessage { get; set; }
    }
}