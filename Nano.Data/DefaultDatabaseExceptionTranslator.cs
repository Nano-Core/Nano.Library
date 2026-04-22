using System;
using Microsoft.EntityFrameworkCore;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Exceptions;
using Nano.Data.Extensions;

namespace Nano.Data;

internal sealed class DefaultDatabaseExceptionTranslator : IDatabaseExceptionTranslator
{
    public Exception Translate(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        if (exception is DbUpdateException dbUpdateException && dbUpdateException.IsUniqueViolation())
        {
            return new UniqueConstraintViolationException(dbUpdateException);
        }

        return exception;
    }
}