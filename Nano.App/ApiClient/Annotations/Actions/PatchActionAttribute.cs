using System.Net.Http;

namespace Nano.App.ApiClient.Annotations.Actions;

/// <summary>
/// Defines an PATCH action of a Nano api-client Request.
/// </summary>
public sealed class PatchActionAttribute : ActionAttribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="PatchActionAttribute"/> with action set
    /// and http method PUT.
    /// </summary>
    /// <param name="action">The action of the request.</param>
    public PatchActionAttribute(string action)
        : base(action, HttpMethod.Patch)
    {
    }
}