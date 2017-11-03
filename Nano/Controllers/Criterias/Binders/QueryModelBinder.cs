using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nano.Controllers.Criterias.Entities;
using Nano.Controllers.Criterias.Enums;
using Nano.Controllers.Criterias.Interfaces;

namespace Nano.Controllers.Criterias.Binders
{
    /// <inheritdoc />
    public class QueryModelBinder<TCriteria> : IModelBinder
        where TCriteria : ICriteria, new()
    {
        /// <inheritdoc />
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            // TODO: MODEL BINDING: Improve Criteria Binding (only querystring is used now!)

            var query = bindingContext.ActionContext.HttpContext.Request.Query;

            var success = int.TryParse(query["number"].FirstOrDefault(), out var number);
            if (!success)
                number = 1;

            success = int.TryParse(query["count"].FirstOrDefault(), out var count);
            if (!success)
                count = 25;

            var orderBy = query["by"].FirstOrDefault() ?? "Id";

            success = Enum.TryParse<SortDirection>(query["direction"].FirstOrDefault(), true, out var direction);
            if (!success)
                direction = SortDirection.Asc;

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
                },
                Criteria = new TCriteria()
            };

            var exclude = new[] { "by", "count", "number", "direction" };

            foreach (var parameter in query.Where(x => !exclude.Contains(x.Key)))
            {
                var type = typeof(TCriteria);
                var property = type.GetProperty(parameter.Key, BindingFlags.IgnoreCase | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);

                if (property == null)
                    continue;

                var value = parameter.Value.FirstOrDefault();

                if (property.PropertyType == typeof(DateTimeOffset) || property.PropertyType == typeof(DateTimeOffset?))
                {
                    property.SetValue(model.Criteria, DateTimeOffset.Parse(value));
                }
                else
                {
                    property.SetValue(model.Criteria, value);
                }
            }

            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }
}