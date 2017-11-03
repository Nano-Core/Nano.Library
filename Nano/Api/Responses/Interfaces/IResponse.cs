namespace Nano.Api.Responses.Interfaces
{
    /// <summary>
    /// Responses interface.
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// Raw Json.
        /// </summary>
        string RawJson { get; set; }

        /// <summary>
        /// Raw Querystring.
        /// </summary>
        string RawQueryString { get; set; }

        /// <summary>
        /// Error Message.
        /// </summary>
        string ErrorMessage { get; set; }
    }
}