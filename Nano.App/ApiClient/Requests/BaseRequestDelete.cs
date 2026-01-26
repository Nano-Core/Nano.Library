namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a base class for DELETE requests.
/// </summary>
public abstract class BaseRequestDelete : BaseRequest
{
    /// <summary>
    /// Gets the body of the request.
    /// </summary>
    public abstract object? GetBody();
}