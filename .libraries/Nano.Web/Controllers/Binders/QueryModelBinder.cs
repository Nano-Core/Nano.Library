using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DynamicExpression.Entities;
using DynamicExpression.Enums;
using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Nano.Web.Controllers.Binders
{
    /// <inheritdoc />
    public class QueryModelBinder<TCriteria> : IModelBinder
        where TCriteria : class, IQueryCriteria, new()
    { 
        /// <inheritdoc />
        public virtual Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var request = bindingContext.ActionContext.HttpContext.Request;

            string body;
            using (var sr = new StreamReader(request.Body))
            {
                body = sr.ReadToEnd();
            }

            var model = string.IsNullOrEmpty(body) 
                ? new Query<TCriteria>()
                : JsonConvert.DeserializeObject<Query<TCriteria>>(body);

            model.Order = model.Order ?? this.GetOrdering(request);
            model.Paging = model.Paging ?? this.GetPagination(request);
            model.Criteria = model.Criteria ?? this.GetCriteria(request);

            bindingContext.Result = ModelBindingResult.Success(model);

            return Task.CompletedTask;
        }

        private TCriteria GetCriteria(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var criteria = new TCriteria();

            typeof(TCriteria)
                .GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance)
                .ToList()
                .ForEach(x =>
                {
                    var success = request.Query.TryGetValue(x.Name, out var values);
                    if (!success)
                        return;

                    var value = values.FirstOrDefault();
                    if (value == null)
                        return;

                    if (x.PropertyType == typeof(TimeSpan) || x.PropertyType == typeof(TimeSpan?))
                    {
                        x.SetValue(criteria, TimeSpan.Parse(value));
                    }
                    else if (x.PropertyType == typeof(DateTimeOffset) || x.PropertyType == typeof(DateTimeOffset?))
                    {
                        x.SetValue(criteria, DateTimeOffset.Parse(value));
                    }
                    else
                    {
                        x.SetValue(criteria, value);
                    }
                });

            return criteria;
        }

        private IOrdering GetOrdering(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var by = this.GetOrderingBy(request);
            var direction = this.GetOrderingDirection(request);

            return new Ordering
            {
                By = by,
                Direction = direction
            };
        }
        private string GetOrderingBy(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var orderBy = request.Query["by"].FirstOrDefault() ?? "Id";

            return orderBy;
        }
        private OrderingDirection GetOrderingDirection(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var success = Enum.TryParse<OrderingDirection>(request.Query["direction"].FirstOrDefault(), true, out var direction);
            if (!success)
                direction = OrderingDirection.Asc;

            return direction;
        }

        private IPagination GetPagination(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var count = this.GetPaginationCount(request);
            var number = this.GetPaginationNumber(request);

            return new Pagination
            {
                Count = count,
                Number = number
            };
        }
        private int GetPaginationCount(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var success = int.TryParse(request.Query["count"].FirstOrDefault(), out var count);
            if (!success)
                count = 25;

            return count;
        }
        private int GetPaginationNumber(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var success = int.TryParse(request.Query["number"].FirstOrDefault(), out var number);
            if (!success)
                number = 1;

            return number;
        }
    }
}