using System;
using System.Data.Common;

namespace Nano.Data.Extensions;

internal static class DbCommandExtensions
{
    public static DbParameter CreateParameter(this DbCommand command, string name, object? value)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(name);

        var parameter = command
            .CreateParameter();

        parameter.ParameterName = name.StartsWith('@')
            ? name
            : "@" + name;

        parameter.Value = value ?? DBNull.Value;

        return parameter;
    }
}