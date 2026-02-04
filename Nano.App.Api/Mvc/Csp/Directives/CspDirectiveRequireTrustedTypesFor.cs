using Nano.App.Api.Mvc.Csp.Directives.Abstractions;

namespace Nano.App.Api.Mvc.Csp.Directives;

internal sealed class CspDirectiveRequireTrustedTypesFor : BaseCspDirective, ICspDirectiveRequireTrustedTypesFor
{
    public override string Name => "require-trusted-types-for";

    public void Script() => this.Values.Add("script");
}