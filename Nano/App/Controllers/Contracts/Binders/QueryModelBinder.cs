using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nano.App.Controllers.Contracts.Enums;
using Nano.App.Controllers.Contracts.Interfaces;
using Serilog;

namespace Nano.App.Controllers.Contracts.Binders
{
    /// <inheritdoc />
    public class QueryModelBinder<TCriteria> : IModelBinder
        where TCriteria : ICriteria, new()
    {
        // TODO: BindModelAsync: Improve structure and reuse binding from Paging and Ordering.
        /// <inheritdoc />
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var type = typeof(TCriteria);
            var query = bindingContext.ActionContext.HttpContext.Request.Query;
       
            var success = int.TryParse(query["number"], out var number);
            if (!success)
                number = 1;

            success = int.TryParse(query["count"], out var count);
            if (!success)
                count = 25;

            var orderBy = query["by"].FirstOrDefault() ?? "Id";

            success = Enum.TryParse<Direction>(query["direction"], true, out var direction);
            if (!success)
                direction = Direction.Asc;

            var model = new Query<TCriteria>
            {
                Paging = new Pagination
                {
                    Count = count,
                    Number = number
                },
                Order = new Ordering
                {
                    By = orderBy,
                    Direction = direction
                }
            };

            var exclude = new[] {"by", "count", "number", "direction"};

            foreach (var parameter in query.Where(x => !exclude.Contains(x.Key)))
            {
                var property = type.GetProperty(parameter.Key, BindingFlags.IgnoreCase | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);

                if (property == null)
                    continue;

                var value = parameter.Value.Single();
                try
                {
                    // TODO: BindModelAsync: add cases for all simple types, they throw exception by not being object.
                    if (property.PropertyType == typeof(DateTimeOffset) || property.PropertyType == typeof(DateTimeOffset?))
                    {
                        property.SetValue(model.Criteria, DateTimeOffset.Parse(value));
                    }
                    else
                    {
                        property.SetValue(model.Criteria, value);
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning($"Property binding failed {type.Name}.{property.Name}: [{value}] - Exception: {ex.Message}");
                }
            }

            bindingContext.Result = ModelBindingResult.Success(model);

            return Task.CompletedTask;
        }
    }
}