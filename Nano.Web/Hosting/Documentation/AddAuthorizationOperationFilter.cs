using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nano.Web.Hosting.Documentation
{
    /// <summary> 
    /// This operation filter inspects the filter descriptors to look for authorization filters and if found, will add a non-body operation parameter that 
    /// requires the user to provide an access token when invoking the api endpoints 
    /// </summary> 
    public class AddAuthorizationOperationFilter : IOperationFilter
    {
        /// <summary> 
        /// </summary> 
        /// <param name="operation"></param> 
        /// <param name="context"></param> 
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var descriptor = context.ApiDescription.ActionDescriptor;
            var isAuthorized = descriptor.FilterDescriptors.Any(x => x.Filter is AuthorizeFilter);
            var allowAnonymous = descriptor.FilterDescriptors.Any(x => x.Filter is AllowAnonymousFilter);

            if (isAuthorized && !allowAnonymous)
            {
                if (operation.Parameters == null)
                {
                    operation.Parameters = new List<IParameter>();
                }
                operation.Parameters.Add(new NonBodyParameter
                {
                    In = "header",
                    Type = "string",
                    Name = "Authorization",
                    Default = "Bearer ",
                    Description = "jwt token",
                    Required = true
                });
            }
        }
    }
}