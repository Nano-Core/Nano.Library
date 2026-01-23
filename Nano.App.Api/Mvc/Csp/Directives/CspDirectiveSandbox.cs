using System.Collections.Generic;
using Nano.App.Api.Mvc.Csp.Directives.Abstractions;

namespace Nano.App.Api.Mvc.Csp.Directives;

internal sealed class CspDirectiveSandbox(List<string> values)
    : BaseCspBuilder(values), ICspDirectiveSandbox
{
    public void AllowDownloads() => this.values.Add("allow-downloads");
    public void AllowForms() => this.values.Add("allow-forms");
    public void AllowModals() => this.values.Add("allow-modals");
    public void AllowOrientationLock() => this.values.Add("allow-orientation-lock");
    public void AllowPointerLock() => this.values.Add("allow-pointer-lock");
    public void AllowPopups() => this.values.Add("allow-popups");
    public void AllowPopupsToEscapeSandbox() => this.values.Add("allow-popups-to-escape-sandbox");
    public void AllowPresentation() => this.values.Add("allow-presentation");
    public void AllowSameOrigin() => this.values.Add("allow-same-origin");
    public void AllowScripts() => this.values.Add("allow-scripts");
    public void AllowStorageAccessByUserActivation() => this.values.Add("allow-storage-access-by-user-activation");
    public void AllowTopNavigation() => this.values.Add("allow-top-navigation");
    public void AllowTopNavigationByUserActivation() => this.values.Add("allow-top-navigation-by-user-activation");
    public void AllowTopNavigationToCustomProtocols() => this.values.Add("allow-top-navigation-to-custom-protocols");
}