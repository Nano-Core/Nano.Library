using System;
using System.IO;
using System.Text;
using Nano.App.Api.Mvc.Documentation.Regex;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Nano.App.Api.Mvc.Documentation.Extensions;

internal static class SwaggerUIOptionsExtensions
{
    internal static SwaggerUIOptions UseCspNonce(this SwaggerUIOptions options, string? cspNonce)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (cspNonce == null)
        {
            return options;
        }

        var originalIndexStreamFactory = options.IndexStream;

        options.IndexStream = () =>
        {
            using var originalStream = originalIndexStreamFactory();
            using var originalStreamReader = new StreamReader(originalStream);

            var originalIndexHtmlContents = originalStreamReader
                .ReadToEnd();

            var replacement = $"<$1$2 nonce=\"{cspNonce}\">";

            var nonceEnabledIndexHtmlContents = Regexes.ScriptAndStyles()
                .Replace(originalIndexHtmlContents, replacement);

            var bytes = Encoding.UTF8
                .GetBytes(nonceEnabledIndexHtmlContents);

            return new MemoryStream(bytes);
        };

        return options;
    }
}