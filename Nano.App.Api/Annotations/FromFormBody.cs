using System;
using Microsoft.AspNetCore.Mvc;
using Nano.App.Api.Mvc.ModelBinders;

namespace Nano.App.Api.Annotations;

/// <summary>
/// Indicates that a parameter should be bound from the request's form body as JSON.
/// Supports scenarios where both files and JSON data are sent in the same request.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class FromFormBodyAttribute : ModelBinderAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FromFormBodyAttribute"/> attribute.
    /// Sets the binder type to <see cref="JsonFormModelBinder"/>.
    /// </summary>
    public FromFormBodyAttribute()
    {
        this.BinderType = typeof(JsonFormModelBinder);
    }
}