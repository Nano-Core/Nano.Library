using System;
using Microsoft.AspNetCore.Mvc;
using Nano.Web.Hosting.ModelBinders;

namespace Nano.Web.Attributes;

/// <summary>
/// From Form Body.
/// Using a specialized <see cref="JsonModelBinder"/>, that allows json post with a form.
/// Use when both files and json body are needed for parameters to a controller action.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class FromFormBody : ModelBinderAttribute
{
    /// <summary>
    /// Constructor
    /// </summary>
    public FromFormBody()
    {
        this.BinderType = typeof(JsonModelBinder);
    }
}