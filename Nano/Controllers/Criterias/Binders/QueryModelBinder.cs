using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nano.Controllers.Criterias.Entities;
using Nano.Controllers.Criterias.Interfaces;
using Newtonsoft.Json;

namespace Nano.Controllers.Criterias.Binders
{
    /// <inheritdoc />
    public class QueryModelBinder<TCriteria> : IModelBinder
        where TCriteria : class, ICriteria, new()
    { 
        /// <inheritdoc />
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var request = bindingContext.ActionContext.HttpContext.Request;

            string body;
            using (var sr = new StreamReader(request.Body))
            {
                body = sr.ReadToEnd();
            }

            var model = string.IsNullOrEmpty(body) 
                ? new Query<TCriteria>()
                : JsonConvert.DeserializeObject<Query<TCriteria>>(body);

            model.Order = model.Order ?? Ordering.Parse(request);
            model.Paging = model.Paging ?? Pagination.Parse(request);
            model.Criteria = model.Criteria ?? DefaultCriteria.Parse<TCriteria>(request);

            bindingContext.Result = ModelBindingResult.Success(model);

            return Task.CompletedTask;
        }
    }
}