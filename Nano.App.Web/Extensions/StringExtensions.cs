using System;

namespace Nano.App.Web.Extensions;

internal static class StringExtensions
{
    internal static string ReplaceAsync(this string methodName)
    {
        if (methodName == null) 
            throw new ArgumentNullException(nameof(methodName));
        
        return methodName
            .Replace("Async", "");
    }
}