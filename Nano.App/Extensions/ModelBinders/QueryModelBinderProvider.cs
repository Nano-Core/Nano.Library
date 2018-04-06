using System;
using DynamicExpression.Entities;
using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nano.Models.Criterias;

namespace Nano.App.Extensions.ModelBinders
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
                return (IModelBinder)Activator.CreateInstance(typeof(QueryModelBinder<DefaultQueryCriteria>));
            }

            return null;
        }
    }
}