using Nano.App.Api.Mvc.Csp.Directives.Abstractions;

namespace Nano.App.Api.Mvc.Csp.Directives;

internal abstract class BaseCspDirectiveSimple : BaseCspDirective, ICspDirectiveSimple
{
    public void None()
    {
        this.Values
            .Clear();

        this.Values
            .Add("'none'");
    }

    public void Self()
    {
        this.Values
            .Add("'self'");
    }

    public void Sources(params string[] sources)
    {
        if (sources.Length > 0)
        {
            this.Values
                .AddRange(sources);
        }
    }

    public void UnsafeInline()
    {
        this.Values
            .Add("'unsafe-inline'");
    }

    public void UnsafeHashes()
    {
        this.Values
            .Add("'unsafe-hashes'");
    }

    public void Nonces(params string[] nonces)
    {
        foreach (var nonce in nonces)
        {
            this.Values
                .Add($"'nonce-{nonce}'");
        }
    }

    public void Hashes(params string[] hashes)
    {
        foreach (var hash in hashes)
        {
            this.Values
                .Add($"'{hash}'");
        }
    }

    public void ReportSample()
    {
        this.Values
            .Add("'report-sample'");
    }
}