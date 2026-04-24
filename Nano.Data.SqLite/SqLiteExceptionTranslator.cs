using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Exceptions;

namespace Nano.Data.SqLite;

internal sealed class SqLiteExceptionTranslator : IDatabaseExceptionTranslator
{
    public Exception Translate(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        if (exception is DbUpdateException { InnerException: SqliteException { SqliteErrorCode: 19 } } dbUpdateException)
        {
            return new UniqueConstraintViolationException(dbUpdateException);
        }

        return exception;
    }
}