using Nano.App.Api.Mvc.Csp.Directives.Abstractions;

namespace Nano.App.Api.Mvc.Csp.Directives;

internal sealed class CspDirectiveSandbox : BaseCspDirectiveSimple, ICspDirectiveSandbox
{
    public override string Name => "sandbox";

    public void AllowDownloads() => this.Values.Add("allow-downloads");
    public void AllowForms() => this.Values.Add("allow-forms");
    public void AllowModals() => this.Values.Add("allow-modals");
    public void AllowOrientationLock() => this.Values.Add("allow-orientation-lock");
    public void AllowPointerLock() => this.Values.Add("allow-pointer-lock");
    public void AllowPopups() => this.Values.Add("allow-popups");
    public void AllowPopupsToEscapeSandbox() => this.Values.Add("allow-popups-to-escape-sandbox");
    public void AllowPresentation() => this.Values.Add("allow-presentation");
    public void AllowSameOrigin() => this.Values.Add("allow-same-origin");
    public void AllowScripts() => this.Values.Add("allow-scripts");
    public void AllowStorageAccessByUserActivation() => this.Values.Add("allow-storage-access-by-user-activation");
    public void AllowTopNavigation() => this.Values.Add("allow-top-navigation");
    public void AllowTopNavigationByUserActivation() => this.Values.Add("allow-top-navigation-by-user-activation");
    public void AllowTopNavigationToCustomProtocols() => this.Values.Add("allow-top-navigation-to-custom-protocols");
}