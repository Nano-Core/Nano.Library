namespace Nano.App.Api.Requests;

/// <summary>
/// Base Request Post.
/// </summary>
public abstract class BaseRequestPost : BaseRequest
{
    /// <summary>
    /// Get Body.
    /// </summary>
    public abstract object GetBody();
}