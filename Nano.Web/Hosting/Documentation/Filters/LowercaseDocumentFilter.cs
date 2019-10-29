using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nano.Web.Hosting.Documentation.Filters
{
    /// <inheritdoc />
    public class LowercaseDocumentFilter : IDocumentFilter
    {
        /// <inheritdoc />
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = swaggerDoc.Paths;

            var newPaths = new Dictionary<string, OpenApiPathItem>();
            var removeKeys = new List<string>();
            foreach (var (key, value) in paths)
            {
                var newKey = key.ToLower();
                if (newKey != key)
                {
                    removeKeys.Add(key);
                    newPaths.Add(newKey, value);
                }
            }

            foreach (var (key, value) in newPaths)
            {
                swaggerDoc.Paths.Add(key, value);
            }

            foreach (var key in removeKeys)
            {
                swaggerDoc.Paths.Remove(key);
            }
        }
    }
}