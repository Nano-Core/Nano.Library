using System;
using Microsoft.EntityFrameworkCore;

namespace Nano.App.Api.Mvc.Extensions;

internal static class DbUpdateExceptionExtensions
{
    internal static bool IsUniqueViolation(this DbUpdateException ex)
    {
        ArgumentNullException.ThrowIfNull(ex);

        var message = ex.ToString();

        return
            IsMySql(message) ||
            IsSqlServer(message) ||
            IsPostgres(message) ||
            IsSqlite(message);
    }

    
    private static bool IsMySql(string message)
    {
        ArgumentNullException.ThrowIfNull(message);

        // Example: "Duplicate entry 'x' for key 'UX_Name'"
        return message.Contains("Duplicate entry", StringComparison.OrdinalIgnoreCase)
               && message.Contains("for key", StringComparison.OrdinalIgnoreCase);
    }
    private static bool IsSqlServer(string message)
    {
        ArgumentNullException.ThrowIfNull(message);

        // Examples: "Cannot insert duplicate key row in object..." "Violation of UNIQUE KEY constraint..."
        return message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) || message.Contains("UNIQUE KEY constraint", StringComparison.OrdinalIgnoreCase);
    }
    private static bool IsPostgres(string message)
    {
        ArgumentNullException.ThrowIfNull(message);

        // Example: "duplicate key value violates unique constraint"
        return message.Contains("violates unique constraint", StringComparison.OrdinalIgnoreCase) || message.Contains("duplicate key value", StringComparison.OrdinalIgnoreCase);
    }
    private static bool IsSqlite(string message)
    {
        ArgumentNullException.ThrowIfNull(message);

        // Example: "UNIQUE constraint failed: Table.Column"
        return message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase);
    }
}