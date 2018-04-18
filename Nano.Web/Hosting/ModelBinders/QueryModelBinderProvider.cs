using System;
using DynamicExpression.Entities;
using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nano.Models.Criterias;

namespace Nano.Web.Hosting.ModelBinders
{
    /// <inheritdoc />
    public class QueryModelBinderProvider : IModelBinderProvider
    {
        /// <inheritdoc />
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var modelType = context.Metadata.ModelType;

            if (modelType.IsGenericType && (modelType.GetGenericTypeDefinition() == typeof(IQuery<>) || modelType.GetGenericTypeDefinition() == typeof(Query<>)))
            {
                var types = modelType.GetGenericArguments();
                var genericType = typeof(QueryModelBinder<>).MakeGenericType(types);

                return (IModelBinder)Activator.CreateInstance(genericType);
            }

            if (modelType == typeof(IQuery) || modelType == typeof(Query))
            {
                var genericType = typeof(QueryModelBinder<DefaultQueryCriteria>);

                return (IModelBinder)Activator.CreateInstance(genericType);
            }

            return null;
        }
    }
}