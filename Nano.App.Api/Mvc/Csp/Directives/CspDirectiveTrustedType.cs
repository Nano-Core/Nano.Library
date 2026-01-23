using System.Collections.Generic;
using Nano.App.Api.Mvc.Csp.Directives.Abstractions;

namespace Nano.App.Api.Mvc.Csp.Directives;

internal sealed class CspDirectiveTrustedType(List<string> values)
    : BaseCspBuilder(values), ICspDirectiveTrustedType
{
    public void AllowDuplicates()
    {
        this.values
            .Add("'allow-duplicates'");
    }

    public void TrustedTypes(params string[] policies)
    {
        foreach (var policy in policies)
        {
            this.values
                .Add(policy);
        }
    }
}