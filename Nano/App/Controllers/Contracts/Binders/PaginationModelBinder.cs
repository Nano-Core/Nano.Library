using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Nano.App.Controllers.Contracts.Binders
{
    /// <inheritdoc />
    public class PaginationModelBinder : IModelBinder
    {
        /// <inheritdoc />
        public virtual Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var query = bindingContext.ActionContext.HttpContext.Request.Query;

            var success = int.TryParse(query["number"], out var number);
            if (!success)
                number = 1;

            success = int.TryParse(query["count"], out var count);
            if (!success)
                count = 25;

            var model = new Pagination
            {
                Count = count,
                Number = number
            };

            bindingContext.Result = ModelBindingResult.Success(model);

            return Task.CompletedTask;
        }
    }
}