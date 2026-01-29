using System.Net.Http;

namespace Nano.App.ApiClient.Annotations.Actions;

/// <summary>
/// Defines an POST action of a Nano api-client Request.
/// </summary>
public sealed class PostActionAttribute : ActionAttribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="PostActionAttribute"/> with action set
    /// and http method POST.
    /// </summary>
    /// <param name="action">The action of the request.</param>
    public PostActionAttribute(string action)
        : base(action, HttpMethod.Post)
    {
    }
}