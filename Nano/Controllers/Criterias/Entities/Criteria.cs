using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Nano.Controllers.Criterias.Interfaces;
using Nano.Models.Interfaces;

namespace Nano.Controllers.Criterias.Entities
{
    /// <summary>
    /// Default Query.
    /// </summary>
    public class Criteria : ICriteria
    {
        /// <summary>
        /// Is Active (read-only).
        /// Default: true.
        /// </summary>
        public virtual bool IsActive { get; } = true;

        /// <summary>
        /// After At.
        /// </summary>
        public virtual DateTimeOffset? AfterAt { get; set; }

        /// <summary>
        /// Before At.
        /// </summary>
        public virtual DateTimeOffset? BeforeAt { get; set; }

        /// <inheritdoc />
        public virtual Filter GetExpression<TEntity>() 
            where TEntity : class, IEntity
        {
            var filter = new Filter();

            filter.Equal("IsActive", this.IsActive);

            if (this.BeforeAt.HasValue)
                filter.LessThanOrEqualTo("CreatedAt", this.BeforeAt);

            if (this.AfterAt.HasValue)
                filter.GreaterThanOrEqualTo("CreatedAt", this.AfterAt);

            return filter;
        }

        /// <summary>
        /// Parse.
        /// </summary>
        /// <typeparam name="TCriteria">The type of <see cref="ICriteria"/>.</typeparam>
        /// <param name="request">The <see cref="HttpRequest"/>.</param>
        /// <returns>An instance of <typeparamref name="TCriteria"/>.</returns>
        public static TCriteria Parse<TCriteria>(HttpRequest request)
            where TCriteria : ICriteria, new()
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
    }
}