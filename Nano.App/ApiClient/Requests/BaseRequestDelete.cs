namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Base Request Delete.
/// </summary>
public abstract class BaseRequestDelete : BaseRequest
{
    /// <summary>
    /// Get Body.
    /// </summary>
    public abstract object? GetBody();
}