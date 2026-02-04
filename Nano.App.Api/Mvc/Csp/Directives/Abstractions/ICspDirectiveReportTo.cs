namespace Nano.App.Api.Mvc.Csp.Directives.Abstractions;

internal interface ICspDirectiveReportTo : ICspDirective
{
    void Group(string name);
}