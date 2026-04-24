using System;
using Microsoft.EntityFrameworkCore;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Exceptions;
using Npgsql;

namespace Nano.Data.PostgreSQL;

internal sealed class PostgreSqlExceptionTranslator : IDatabaseExceptionTranslator
{
    public Exception Translate(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        if (exception is DbUpdateException { InnerException: PostgresException { SqlState: "23505" } } dbUpdateException)
        {
            return new UniqueConstraintViolationException(dbUpdateException);
        }

        return exception;
    }
}