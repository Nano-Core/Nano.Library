namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Base Request Post.
/// </summary>
public abstract class BaseRequestPost : BaseRequest
{
    /// <summary>
    /// Get Body.
    /// </summary>
    public abstract object? GetBody();
}