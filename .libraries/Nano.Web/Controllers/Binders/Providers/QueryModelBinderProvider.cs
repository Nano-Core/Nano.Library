using System;
using System.Reflection;
using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nano.Models.Criterias;

namespace Nano.Web.Controllers.Binders.Providers
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
            {
                return (IModelBinder)Activator.CreateInstance(typeof(QueryModelBinder<DefaultQueryCriteria>));
            }

            if (modelType.GetTypeInfo().IsGenericType && modelType.GetGenericTypeDefinition() == typeof(IQuery<>))
            {
                var types = modelType.GetGenericArguments();
                var genericType = typeof(QueryModelBinder<>).MakeGenericType(types);

                return (IModelBinder)Activator.CreateInstance(genericType);
            }

            return null;
        }
    }
}