using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Nano.App.Controllers.Criteria.Binders.Providers
{
    /// <inheritdoc />
    public class CriteriaModelBinderProvider : IModelBinderProvider
    {
        /// <inheritdoc />
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var modelType = context.Metadata.ModelType;

            if (modelType.GetTypeInfo().IsGenericType && modelType.GetGenericTypeDefinition() == typeof(Criteria<>))
            {
                var types = context.Metadata.ModelType.GetGenericArguments();
                var genericType = typeof(CriteriaModelBinder<>).MakeGenericType(types);

                return (IModelBinder)Activator.CreateInstance(genericType);
            }

            return null;
        }
    }
}