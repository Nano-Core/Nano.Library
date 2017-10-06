using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Nano.App.Controllers.Contracts.Binders.Providers
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

            if (modelType.GetTypeInfo().IsGenericType && modelType.GetGenericTypeDefinition() == typeof(Query<>))
            {
                var types = context.Metadata.ModelType.GetGenericArguments();
                var genericType = typeof(QueryModelBinder<>).MakeGenericType(types);

                return (IModelBinder)Activator.CreateInstance(genericType);
            }

            return null;
        }
    }
}