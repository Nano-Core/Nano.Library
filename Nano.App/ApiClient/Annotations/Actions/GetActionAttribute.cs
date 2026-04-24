using System.Net.Http;

namespace Nano.App.ApiClient.Annotations.Actions;

/// <summary>
/// Defines an GET action of a Nano api-client Request.
/// </summary>
public sealed class GetActionAttribute : ActionAttribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="GetActionAttribute"/> with action set
    /// and http method GET.
    /// </summary>
    /// <param name="action">The action of the request.</param>
    public GetActionAttribute(string action)
        : base(action, HttpMethod.Get)
    {
    }
}