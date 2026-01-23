using System;
using System.Collections.Generic;

namespace Nano.App.Api.Mvc.Csp.Directives;

internal abstract class BaseCspBuilder(List<string> values)
{
    protected readonly List<string> values = values ?? throw new ArgumentNullException(nameof(values));

    public void None()
    {
        this.values
            .Clear();

        this.values
            .Add("'none'");
    }

    public void Self()
    {
        this.values
            .Add("'self'");
    }

    public void Sources(params string[] sources)
    {
        if (sources.Length > 0)
        {
            this.values
                .AddRange(sources);
        }
    }

    public void Nonces(params string[] nonces)
    {
        foreach (var nonce in nonces)
        {
            this.values.Add($"'nonce-{nonce}'");
        }
    }

    public void Hashes(params string[] hashes)
    {
        foreach (var hash in hashes)
        {
            this.values.Add($"'{hash}'");
        }
    }

    public void UnsafeInline() => this.values.Add("'unsafe-inline'");
}