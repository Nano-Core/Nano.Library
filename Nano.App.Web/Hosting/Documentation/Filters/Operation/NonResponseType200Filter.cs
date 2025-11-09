using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Nano.Models.Const;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nano.Web.Hosting.Documentation.Filters.Operation;

/// <summary>
/// Non 200 Responses Filter.
/// </summary>
public class NonResponseType200Filter : IOperationFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var response in operation.Responses)
        {
            var statusCode = Convert.ToInt32(response.Key);

            switch (statusCode)
            {
                case (int)HttpStatusCode.OK:
                case (int)HttpStatusCode.Created:
                case (int)HttpStatusCode.Accepted:
                case (int)HttpStatusCode.NoContent:
                case (int)HttpStatusCode.ResetContent:
                case (int)HttpStatusCode.PartialContent:
                case (int)HttpStatusCode.MultiStatus:
                case (int)HttpStatusCode.AlreadyReported:
                case (int)HttpStatusCode.IMUsed:
                    continue;
            }

            var responseType = context.ApiDescription.SupportedResponseTypes
                .FirstOrDefault(x => x.StatusCode == statusCode);

            if (responseType?.Type == null || responseType.Type == typeof(ProblemDetails))
            {
                response.Value.Content
                    .Clear();
                
                response.Value.Content[HttpContentType.JSON] = new OpenApiMediaType();
            }
            else
            {
                OpenApiSchema schema = null;

                var hasValue = response.Value.Content
                    .TryGetValue(HttpContentType.JSON, out var value);

                if (hasValue)
                {
                    schema = value.Schema;
                }
                else if (response.Value.Content.Any())
                {
                    var keyValuePair = response.Value.Content
                        .First();

                    schema = keyValuePair.Value.Schema;
                }

                response.Value.Content
                    .Clear();
                
                response.Value.Content[HttpContentType.JSON] = new OpenApiMediaType
                {
                    Schema = schema
                };
            }
        }
    }
}