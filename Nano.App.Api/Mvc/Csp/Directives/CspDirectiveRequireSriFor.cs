using Nano.App.Api.Mvc.Csp.Directives.Abstractions;

namespace Nano.App.Api.Mvc.Csp.Directives;

internal sealed class CspDirectiveRequireSriFor : BaseCspDirective, ICspDirectiveRequireSriFor
{
    public override string Name => "require-sri-for";

    public void Script() => this.Values.Add("script");
    public void Style() => this.Values.Add("style");
}