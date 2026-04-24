using Nano.App.Api.Mvc.Csp.Directives.Abstractions;

namespace Nano.App.Api.Mvc.Csp.Directives;

internal class CspDirectiveReportTo : BaseCspDirective, ICspDirectiveReportTo
{
    public override string Name => "report-to";

    public void Group(string name) => this.Values.Add(name);
}