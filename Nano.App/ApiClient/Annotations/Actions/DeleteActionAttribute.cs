using System.Net.Http;

namespace Nano.App.ApiClient.Annotations.Actions;

/// <summary>
/// Defines an DELETE action of a Nano api-client Request.
/// </summary>
public sealed class DeleteActionAttribute : ActionAttribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="DeleteActionAttribute"/> with action set
    /// and http method DELETE.
    /// </summary>
    /// <param name="action">The action of the request.</param>
    public DeleteActionAttribute(string action)
        : base(action, HttpMethod.Delete)
    {
    }
}