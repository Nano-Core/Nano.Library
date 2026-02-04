using Nano.App.Api.Mvc.Csp.Directives.Abstractions;

namespace Nano.App.Api.Mvc.Csp.Directives;

internal sealed class CspDirectiveTrustedTypes : BaseCspDirectiveSimple, ICspDirectiveTrustedTypes
{
    public override string Name => "trusted-types";

    public void AllowDuplicates()
    {
        this.Values
            .Add("'allow-duplicates'");
    }

    public void TrustedTypes(params string[] policies)
    {
        foreach (var policy in policies)
        {
            this.Values
                .Add(policy);
        }
    }
}