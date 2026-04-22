using System;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Exceptions;

namespace Nano.Data.MySql;

internal sealed class MySqlExceptionTranslator : IDatabaseExceptionTranslator
{
    public Exception Translate(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        if (exception is DbUpdateException { InnerException: MySqlException { Number: 1062 } } dbUpdateException)
        {
            return new UniqueConstraintViolationException(dbUpdateException);
        }

        return exception;
    }
}