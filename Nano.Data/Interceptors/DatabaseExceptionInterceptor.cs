using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Nano.Data.Abstractions;

namespace Nano.Data.Interceptors;

internal sealed class DatabaseExceptionInterceptor(IDatabaseExceptionTranslator translator) : SaveChangesInterceptor
{
    private readonly IDatabaseExceptionTranslator translator = translator ?? throw new ArgumentNullException(nameof(translator));

    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        throw this.translator
            .Translate(eventData.Exception);
    }

    public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        throw this.translator
            .Translate(eventData.Exception);
    }
}