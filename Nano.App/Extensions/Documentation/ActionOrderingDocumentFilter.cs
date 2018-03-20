using System.Linq;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nano.App.Extensions.Documentation
{
    /// <inheritdoc />
    public class ActionOrderingDocumentFilter : IDocumentFilter
    {
        /// <inheritdoc />
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = swaggerDoc.Paths
                .OrderBy(x => x.Key)
                .ToDictionary(e => e.Key, e => e.Value);

            swaggerDoc.Paths = paths;
        }
    }
}