using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Nano.App.Controllers.Contracts.Binders.Providers
{
    /// <inheritdoc />
    public class PaginationModelBinderProvider : IModelBinderProvider
    {
        /// <inheritdoc />
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var modelType = context.Metadata.ModelType;

            if (modelType == typeof(Pagination))
            {
                return (IModelBinder)Activator.CreateInstance(typeof(PaginationModelBinder));
            }

            return null;
        }
    }
}