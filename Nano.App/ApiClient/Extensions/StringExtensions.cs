using System;

namespace Nano.App.ApiClient.Extensions;

internal static class StringExtensions
{
    internal static string SmartFormat(this string template, params object[] values)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(values);

        var result = template;

        foreach (var value in values)
        {
            var start = result
                .IndexOf('{');

            if (start < 0)
            {
                break;
            }

            var end = result
                .IndexOf('}', start);

            if (end < 0)
            {
                break;
            }

            result = result[..start] + value + result[(end + 1)..];
        }

        return result;
    }
}