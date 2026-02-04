namespace Nano.App.Api.Mvc.Csp.Directives;

internal class CspDirectiveUpgradeInsecureRequests : BaseCspDirective
{
    public override string Name => "upgrade-insecure-requests";

    public CspDirectiveUpgradeInsecureRequests()
    {
        this.Values
            .Add("");
    }
}