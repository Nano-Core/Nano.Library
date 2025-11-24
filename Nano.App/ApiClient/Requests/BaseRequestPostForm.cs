using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nano.App.ApiClient.Requests.Attributes;
using Nano.App.ApiClient.Requests.Types;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Base Request Post Form.
/// </summary>
public abstract class BaseRequestPostForm : BaseRequest
{
    /// <summary>
    /// Get Form.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<FormItem> GetForm()
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