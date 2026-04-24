using System.Net.Http;

namespace Nano.App.ApiClient.Annotations.Actions;

/// <summary>
/// Defines an QUERY action of a Nano api-client Request.
/// </summary>
public sealed class QueryActionAttribute : ActionAttribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="QueryActionAttribute"/> with action set
    /// and http method QUERY.
    /// </summary>
    /// <param name="action">The action of the request.</param>
    public QueryActionAttribute(string action)
        : base(action, HttpMethod.Query)
    {
    }
}