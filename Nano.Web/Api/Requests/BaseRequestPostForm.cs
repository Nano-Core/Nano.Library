﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nano.Web.Api.Requests.Attributes;
using Nano.Web.Api.Requests.Types;

namespace Nano.Web.Api.Requests
{
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
                    var type = x.PropertyType;
                    var value = x.GetValue(this);

                    return new FormItem
                    {
                        Name = x.Name,
                        Value = value,
                        Type = type
                    };
                });
        }
    }
}