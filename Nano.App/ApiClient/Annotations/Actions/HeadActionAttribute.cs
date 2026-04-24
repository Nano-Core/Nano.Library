using System.Net.Http;

namespace Nano.App.ApiClient.Annotations.Actions;

/// <summary>
/// Defines an HEAD action of a Nano api-client Request.
/// </summary>
public sealed class HeadActionAttribute : ActionAttribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="HeadActionAttribute"/> with action set
    /// and http method HEAD.
    /// </summary>
    /// <param name="action">The action of the request.</param>
    public HeadActionAttribute(string action)
        : base(action, HttpMethod.Head)
    {
    }
}