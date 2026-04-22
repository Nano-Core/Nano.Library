using System;
using Microsoft.EntityFrameworkCore;

namespace Nano.Data.Extensions;

internal static class DbUpdateExceptionExtensions
{
    internal static bool IsUniqueViolation(this DbUpdateException ex)
    {
        ArgumentNullException.ThrowIfNull(ex);

        var message = ex.ToString();

        return message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) ||
               message.Contains("unique", StringComparison.OrdinalIgnoreCase) ||
               message.Contains("Duplicate entry", StringComparison.OrdinalIgnoreCase) ||
               message.Contains("violates unique constraint", StringComparison.OrdinalIgnoreCase) ||
               message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase) ||
               message.Contains("UNIQUE KEY constraint", StringComparison.OrdinalIgnoreCase) ||
               message.Contains("for key", StringComparison.OrdinalIgnoreCase);
    }
}