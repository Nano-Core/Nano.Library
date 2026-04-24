using System;

namespace Nano.App.Api.Mvc.Extensions;

internal static class StringExtensions
{
    internal static string ReplaceAsync(this string methodName)
    {
        ArgumentNullException.ThrowIfNull(methodName);

        return methodName
            .Replace("Async", "");
    }
}