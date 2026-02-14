using Nano.App.Api.Mvc.Csp.Directives;
using Nano.App.Api.Mvc.Csp.Directives.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace Nano.App.Api.Mvc.Csp;

internal sealed class CspBuilder
{
    private readonly List<ICspDirective> directives = [];

    internal ICspDirectiveReportTo ReportTo() => this.AddDirective<CspDirectiveReportTo>();
    internal ICspDirective UpgradeInsecureRequests() => this.AddDirective<CspDirectiveUpgradeInsecureRequests>();
    internal ICspDirectiveDefault DefaultSrc() => this.AddDirective<CspDirectiveDefault>();
    internal ICspDirectiveScript ScriptSrc() => this.AddDirective<CspDirectiveScript>();
    internal ICspDirectiveScriptElem ScriptSrcElem() => this.AddDirective<CspDirectiveScriptElem>();
    internal ICspDirectiveScriptAttr ScriptSrcAttr() => this.AddDirective<CspDirectiveScriptAttr>();
    internal ICspDirectiveStyle StyleSrc() => this.AddDirective<CspDirectiveStyle>();
    internal ICspDirectiveStyleElem StyleSrcElem() => this.AddDirective<CspDirectiveStyleElem>();
    internal ICspDirectiveStyleAttr StyleSrcAttr() => this.AddDirective<CspDirectiveStyleAttr>();
    internal ICspDirectiveSimple ObjectSrc() => this.AddDirective<CspDirectiveObject>();
    internal ICspDirectiveSimple ImageSrc() => this.AddDirective<CspDirectiveImage>();
    internal ICspDirectiveSimple MediaSrc() => this.AddDirective<CspDirectiveMedia>();
    internal ICspDirectiveSimple FramesSrc() => this.AddDirective<CspDirectiveFrames>();
    internal ICspDirectiveSimple FencedFramesSrc() => this.AddDirective<CspDirectiveFencedFrames>();
    internal ICspDirectiveSimple FrameAncestorsSrc() => this.AddDirective<CspDirectiveFrameAncestors>();
    internal ICspDirectiveSimple FontSrc() => this.AddDirective<CspDirectiveFont>();
    internal ICspDirectiveSimple ConnectionSrc() => this.AddDirective<CspDirectiveConnection>();
    internal ICspDirectiveSimple BaseUri() => this.AddDirective<CspDirectiveBaseUri>();
    internal ICspDirectiveSimple ChildrenSrc() => this.AddDirective<CspDirectiveChildren>();
    internal ICspDirectiveSimple FormSrc() => this.AddDirective<CspDirectiveForm>();
    internal ICspDirectiveSimple ManifestSrc() => this.AddDirective<CspDirectiveManifest>();
    internal ICspDirectiveSimple WorkerSrc() => this.AddDirective<CspDirectiveWorker>();
    internal ICspDirectiveSimple NavigateTo() => this.AddDirective<CspDirectiveNavigateTo>();
    internal ICspDirectiveSandbox Sandbox() => this.AddDirective<CspDirectiveSandbox>();
    internal ICspDirectiveTrustedTypes TrustedTypes() => this.AddDirective<CspDirectiveTrustedTypes>();
    internal ICspDirectiveRequireTrustedTypesFor RequireTrustedTypesFor() => this.AddDirective<CspDirectiveRequireTrustedTypesFor>();
    internal ICspDirectiveRequireSriFor RequireSriFor() => this.AddDirective<CspDirectiveRequireSriFor>();

    internal string Build()
    {
        var values = this.directives
            .Select(x =>
            {
                if (x.Values.Count == 0)
                {
                    return null;
                }

                return $"{x.Name} {string.Join(' ', x.Values)}".Trim();
            })
            .Where(x => x != null);

        return $"{string.Join("; ", values)};";
    }


    private T AddDirective<T>()
        where T : ICspDirective, new()
    {
        var directive = new T();

        this.directives
            .Add(directive);

        return directive;
    }
}