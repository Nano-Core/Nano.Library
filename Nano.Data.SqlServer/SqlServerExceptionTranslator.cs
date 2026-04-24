using System;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Exceptions;

namespace Nano.Data.SqlServer;

internal sealed class SqlServerExceptionTranslator : IDatabaseExceptionTranslator
{
    public Exception Translate(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        if (exception is DbUpdateException { InnerException: SqlException { Number: 2601 or 2627 } } dbUpdateException)
        {
            return new UniqueConstraintViolationException(dbUpdateException);
        }

        return exception;
    }
}