using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nano.Common.Serialization.Json;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Nano.App.ApiClient.Annotations.Actions;

/// <summary>
/// Defines the action of a Nano api-client Request.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ActionAttribute : Attribute
{
    /// <summary>
    /// The action template of the request.
    /// </summary>
    public string ActionTemplate { get; }

    /// <summary>
    /// The HTTP method of the request.
    /// </summary>
    public HttpMethod Method { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ActionAttribute"/> with action and method set.
    /// </summary>
    /// <param name="action">The action of the request.</param>
    /// <param name="method">The http method of the request.</param>
    public ActionAttribute(string action, HttpMethod method)
    {
        this.ActionTemplate = action ?? throw new ArgumentNullException(nameof(action));
        this.Method = method ?? throw new ArgumentNullException(nameof(method));
    }
}