using System;
using BrunoZell.ModelBinding;
using Microsoft.AspNetCore.Mvc;

namespace Nano.Web.Attributes
{
    /// <summary>
    /// From Form Body.
    /// Using a specialized <see cref="JsonModelBinder"/>, that allows json post with a form.
    /// Use when both files and json body are needed for parameters to a controller action.
    /// https://github.com/BrunoZell/JsonModelBinder
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
}
