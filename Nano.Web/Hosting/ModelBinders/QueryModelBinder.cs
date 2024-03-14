using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DynamicExpression.Entities;
using DynamicExpression.Enums;
using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nano.Models.Extensions;
using Nano.Models.Serialization.Json.Const;
using Newtonsoft.Json;

namespace Nano.Web.Hosting.ModelBinders;

/// <inheritdoc />
public class QueryModelBinder : IModelBinder
{
    /// <inheritdoc />
    public virtual async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
            throw new ArgumentNullException(nameof(bindingContext));

        var request = bindingContext.ActionContext.HttpContext.Request;
        var body = await request.Body.ReadAllAsync();

        var model = string.IsNullOrEmpty(body)
            ? new Query
            {
                Order = this.GetOrdering(request),
                Paging = this.GetPagination(request)
            }
            : JsonConvert.DeserializeObject(body, Globals.GetJsonSerializerSettings());

        bindingContext.Result = ModelBindingResult.Success(model);
    }

    /// <summary>
    /// Returns the <see cref="Ordering"/> from the <see cref="HttpRequest.Query"/>.
    /// </summary>
    /// <param name="request">The <see cref="HttpRequest"/>.</param>
    /// <returns>The <see cref="Ordering"/>.</returns>
    protected virtual Ordering GetOrdering(HttpRequest request)
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

    /// <summary>
    /// Returns the <see cref="Pagination"/> from the <see cref="HttpRequest.Query"/>.
    /// </summary>
    /// <param name="request">The <see cref="HttpRequest"/>.</param>
    /// <returns>The <see cref="Pagination"/>.</returns>
    protected virtual Pagination GetPagination(HttpRequest request)
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

    private string GetOrderingBy(HttpRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var orderBy = request.Query["Order.By"].FirstOrDefault() ?? "Id";

        return orderBy;
    }
    private int GetPaginationCount(HttpRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var success = int.TryParse(request.Query["Paging.Count"].FirstOrDefault(), out var count);
        if (!success)
        {
            count = 25;
        }

        return count;
    }
    private int GetPaginationNumber(HttpRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var success = int.TryParse(request.Query["Paging.Number"].FirstOrDefault(), out var number);
        if (!success)
        {
            number = 1;
        }

        return number;
    }
    private OrderingDirection GetOrderingDirection(HttpRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var success = Enum.TryParse<OrderingDirection>(request.Query["Order.Direction"].FirstOrDefault(), true, out var direction);
        if (!success)
        {
            direction = OrderingDirection.Asc;
        }

        return direction;
    }
}

/// <inheritdoc />
public class QueryModelBinder<TCriteria> : QueryModelBinder
    where TCriteria : class, IQueryCriteria, new()
{
    /// <inheritdoc />
    public override async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
            throw new ArgumentNullException(nameof(bindingContext));

        var request = bindingContext.ActionContext.HttpContext.Request;
        var body = await request.Body.ReadAllAsync();

        var model = string.IsNullOrEmpty(body)
            ? new Query<TCriteria>
            {
                Order = this.GetOrdering(request),
                Paging = this.GetPagination(request),
                Criteria = this.GetCriteria(request)
            }
            : JsonConvert.DeserializeObject(body, Globals.GetJsonSerializerSettings());

        bindingContext.Result = ModelBindingResult.Success(model);
    }

    /// <summary>
    /// Returns the Criteria of type <typeparamref name="TCriteria"/>, from the <see cref="HttpRequest.Query"/>.
    /// </summary>
    /// <param name="request">The <see cref="HttpRequest"/>.</param>
    /// <returns>The Criteria of type <typeparamref name="TCriteria"/>.</returns>
    protected virtual TCriteria GetCriteria(HttpRequest request)
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
                {
                    return;
                }

                var value = values.FirstOrDefault();
                if (value == null)
                {
                    return;
                }

                if (x.PropertyType == typeof(TimeSpan) || x.PropertyType == typeof(TimeSpan?))
                {
                    x.SetValue(criteria, TimeSpan.Parse(value));
                }
                else if (x.PropertyType == typeof(DateTime) || x.PropertyType == typeof(DateTime?))
                {
                    x.SetValue(criteria, DateTime.Parse(value));
                }
                else if (x.PropertyType == typeof(DateTimeOffset) || x.PropertyType == typeof(DateTimeOffset?))
                {
                    x.SetValue(criteria, DateTimeOffset.Parse(value));
                }
                else if (x.PropertyType == typeof(Guid) || x.PropertyType == typeof(Guid?))
                {
                    x.SetValue(criteria, Guid.Parse(value));
                }
                else
                {
                    x.SetValue(criteria, value);
                }
            });

        return criteria;
    }
}