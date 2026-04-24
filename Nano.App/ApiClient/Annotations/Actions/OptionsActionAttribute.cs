using System.Net.Http;

namespace Nano.App.ApiClient.Annotations.Actions;

/// <summary>
/// Defines an OPTIONS action of a Nano api-client Request.
/// </summary>
public sealed class OptionsActionAttribute : ActionAttribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="OptionsActionAttribute"/> with action set
    /// and http method OPTIONS.
    /// </summary>
    /// <param name="action">The action of the request.</param>
    public OptionsActionAttribute(string action)
        : base(action, HttpMethod.Options)
    {
    }
}