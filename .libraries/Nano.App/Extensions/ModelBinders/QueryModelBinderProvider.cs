using System;
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

            if (modelType == typeof(IQuery<>))
                return (IModelBinder)Activator.CreateInstance(typeof(QueryModelBinder<DefaultQueryCriteria>));

            if (modelType.IsGenericType && modelType.GetGenericTypeDefinition() == typeof(IQuery<>))
            {
                var types = modelType.GetGenericArguments();
                var genericType = typeof(QueryModelBinder<>).MakeGenericType(types);

                return (IModelBinder)Activator.CreateInstance(genericType);
            }

            return null;
        }
    }
}