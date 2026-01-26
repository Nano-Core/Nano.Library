using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nano.App.ApiClient.Requests.Annotations;
using Nano.App.ApiClient.Requests.Models;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a base class for POST requests with form data.
/// </summary>
public abstract class BaseRequestPostForm : BaseRequest
{
    internal virtual IEnumerable<FormItem> GetForm()
    {
        return this
            .GetType()
            .GetProperties()
            .Where(x => x.GetCustomAttribute<FormAttribute>() != null)
            .Select(x =>
            {
                var value = x.GetValue(this);

                return new FormItem
                {
                    Name = x.Name,
                    Value = value
                };
            });
    }
}