namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a base class for POST requests.
/// </summary>
public abstract class BaseRequestPost : BaseRequest
{
    /// <summary>
    /// Gets the body of the request.
    /// </summary>
    public abstract object? GetBody();
}