using System.Net.Http;

namespace Nano.App.ApiClient.Annotations.Actions;

/// <summary>
/// Defines an PUT action of a Nano api-client Request.
/// </summary>
public sealed class PutActionAttribute : ActionAttribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="PutActionAttribute"/> with action set
    /// and http method PUT.
    /// </summary>
    /// <param name="action">The action of the request.</param>
    public PutActionAttribute(string action)
        : base(action, HttpMethod.Put)
    {
    }
}