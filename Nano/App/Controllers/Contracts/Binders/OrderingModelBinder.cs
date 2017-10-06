using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nano.App.Controllers.Contracts.Enums;

namespace Nano.App.Controllers.Contracts.Binders
{
    /// <inheritdoc />
    public class OrderingModelBinder : IModelBinder
    {
        /// <inheritdoc />
        public virtual Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var query = bindingContext.ActionContext.HttpContext.Request.Query;
            var orderBy = query["by"].FirstOrDefault() ?? "Id";

            var success = Enum.TryParse<Direction>(query["direction"], true, out var direction);
            if (!success)
                direction = Direction.Asc;

            var model = new Ordering
            {
                By = orderBy,
                Direction = direction
            };

            bindingContext.Result = ModelBindingResult.Success(model);

            return Task.CompletedTask;
        }
    }
}