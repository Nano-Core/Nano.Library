namespace Nano.App.Api.Mvc.Csp.Directives.Abstractions;

internal interface ICspDirectiveSandbox
{
    void AllowDownloads();
    void AllowForms();
    void AllowModals();
    void AllowOrientationLock();
    void AllowPointerLock();
    void AllowPopups();
    void AllowPopupsToEscapeSandbox();
    void AllowPresentation();
    void AllowSameOrigin();
    void AllowScripts();
    void AllowStorageAccessByUserActivation();
    void AllowTopNavigation();
    void AllowTopNavigationByUserActivation();
    void AllowTopNavigationToCustomProtocols();
}