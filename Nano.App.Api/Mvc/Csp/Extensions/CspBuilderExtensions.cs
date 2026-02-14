using System;
using Nano.App.Api.Config;

namespace Nano.App.Api.Mvc.Csp.Extensions;

internal static class CspBuilderExtensions
{
    internal static CspBuilder UseUpgradeInsecureRequests(this CspBuilder builder, CspOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        if (options.UpgradeInsecureRequests)
        {
            builder
                .UpgradeInsecureRequests();
        }

        return builder;
    }

    internal static CspBuilder UseCspReportTo(this CspBuilder builder, CspOptions.CspReportToOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        var reportTo = builder
            .ReportTo();

        reportTo
            .Group(options.Group);

        return builder;
    }

    internal static CspBuilder UseCspDefault(this CspBuilder builder, CspOptions.CspDirectiveOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        var defaultSrc = builder
            .DefaultSrc();

        if (options.IsNone)
        {
            defaultSrc
                .None();
        }
        else
        {
            if (options.IsSelf)
            {
                defaultSrc
                    .Self();
            }

            if (options.Sources.Length > 0)
            {
                defaultSrc
                    .Sources(options.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspScript(this CspBuilder builder, CspOptions.CspDirectiveScriptsOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        var scriptSrc = builder
            .ScriptSrc();

        if (options.IsNone)
        {
            scriptSrc
                .None();
        }
        else
        {
            if (options.IsSelf)
            {
                scriptSrc
                    .Self();
            }

            if (options.IsUnsafeEval)
            {
                scriptSrc
                    .UnsafeEval();
            }

            if (options.IsUnsafeWasmEval)
            {
                scriptSrc
                    .UnsafeWasmEval();
            }

            if (options.IsTrustedTypesEval)
            {
                scriptSrc
                    .TrustedTypesEval();
            }

            if (options.StrictDynamic)
            {
                scriptSrc
                    .StrictDynamic();
            }

            if (options.IsUnsafeHashes)
            {
                scriptSrc
                    .UnsafeHashes();
            }

            if (options.IsUnsafeInline)
            {
                scriptSrc
                    .UnsafeInline();
            }

            if (options.UnsafeHashedAttributes)
            {
                scriptSrc
                    .UnsafeHashedAttributes();
            }

            if (options.UnsafeAllowRedirects)
            {
                scriptSrc
                    .UnsafeAllowRedirects();
            }

            if (options.InlineSpeculationRules)
            {
                scriptSrc
                    .InlineSpeculationRules();
            }

            if (options.Sources.Length > 0)
            {
                scriptSrc
                    .Sources(options.Sources);
            }

            if (options.Nonces.Length > 0)
            {
                scriptSrc
                    .Nonces(options.Nonces);
            }

            if (options.Hashes.Length > 0)
            {
                scriptSrc
                    .Hashes(options.Hashes);
            }

            if (options.ReportSample)
            {
                scriptSrc
                    .ReportSample();
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspScriptElem(this CspBuilder builder, CspOptions.CspDirectiveScriptsElemOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        var scriptSrcElem = builder
            .ScriptSrcElem();

        if (options.IsNone)
        {
            scriptSrcElem
                .None();
        }
        else
        {
            if (options.IsSelf)
            {
                scriptSrcElem
                    .Self();
            }

            if (options.IsUnsafeEval)
            {
                scriptSrcElem
                    .UnsafeEval();
            }

            if (options.IsUnsafeWasmEval)
            {
                scriptSrcElem
                    .UnsafeWasmEval();
            }

            if (options.IsTrustedTypesEval)
            {
                scriptSrcElem
                    .TrustedTypesEval();
            }

            if (options.StrictDynamic)
            {
                scriptSrcElem
                    .StrictDynamic();
            }

            if (options.IsUnsafeInline)
            {
                scriptSrcElem
                    .UnsafeInline();
            }

            if (options.UnsafeAllowRedirects)
            {
                scriptSrcElem
                    .UnsafeAllowRedirects();
            }

            if (options.InlineSpeculationRules)
            {
                scriptSrcElem
                    .InlineSpeculationRules();
            }

            if (options.Sources.Length > 0)
            {
                scriptSrcElem
                    .Sources(options.Sources);
            }

            if (options.Nonces.Length > 0)
            {
                scriptSrcElem
                    .Nonces(options.Nonces);
            }

            if (options.Hashes.Length > 0)
            {
                scriptSrcElem
                    .Hashes(options.Hashes);
            }

            if (options.ReportSample)
            {
                scriptSrcElem
                    .ReportSample();
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspScriptAttr(this CspBuilder builder, CspOptions.CspDirectiveScriptsAttrOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        var scriptSrcAttr = builder
            .ScriptSrcAttr();

        if (options.IsNone)
        {
            scriptSrcAttr
                .None();
        }
        else
        {
            if (options.IsSelf)
            {
                scriptSrcAttr
                    .Self();
            }

            if (options.IsUnsafeInline)
            {
                scriptSrcAttr
                    .UnsafeInline();
            }

            if (options.IsUnsafeHashes)
            {
                scriptSrcAttr
                    .UnsafeHashes();
            }

            if (options.UnsafeHashedAttributes)
            {
                scriptSrcAttr
                    .UnsafeHashedAttributes();
            }

            if (options.Sources.Length > 0)
            {
                scriptSrcAttr
                    .Sources(options.Sources);
            }

            if (options.ReportSample)
            {
                scriptSrcAttr
                    .ReportSample();
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspStyle(this CspBuilder builder, CspOptions.CspDirectiveStylesOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        var styleSrc = builder
            .StyleSrc();

        if (options.IsNone)
        {
            styleSrc
                .None();
        }
        else
        {
            if (options.IsSelf)
            {
                styleSrc
                    .Self();
            }

            if (options.IsUnsafeInline)
            {
                styleSrc
                    .UnsafeInline();
            }

            if (options.IsUnsafeHashes)
            {
                styleSrc
                    .UnsafeHashes();
            }

            if (options.Sources.Length > 0)
            {
                styleSrc
                    .Sources(options.Sources);
            }

            if (options.Nonces.Length > 0)
            {
                styleSrc
                    .Nonces(options.Nonces);
            }

            if (options.Hashes.Length > 0)
            {
                styleSrc
                    .Hashes(options.Hashes);
            }

            if (options.ReportSample)
            {
                styleSrc
                    .ReportSample();
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspStyleElem(this CspBuilder builder, CspOptions.CspDirectiveStylesElemOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        var styleSrcElem = builder
            .StyleSrcElem();

        if (options.IsNone)
        {
            styleSrcElem
                .None();
        }
        else
        {
            if (options.IsSelf)
            {
                styleSrcElem
                    .Self();
            }

            if (options.IsUnsafeInline)
            {
                styleSrcElem
                    .UnsafeInline();
            }

            if (options.Sources.Length > 0)
            {
                styleSrcElem
                    .Sources(options.Sources);
            }

            if (options.Nonces.Length > 0)
            {
                styleSrcElem
                    .Nonces(options.Nonces);
            }

            if (options.Hashes.Length > 0)
            {
                styleSrcElem
                    .Hashes(options.Hashes);
            }

            if (options.ReportSample)
            {
                styleSrcElem
                    .ReportSample();
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspStyleAttr(this CspBuilder builder, CspOptions.CspDirectiveStylesAttrOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        var styleSrcAttr = builder
            .StyleSrcAttr();

        if (options.IsNone)
        {
            styleSrcAttr
                .None();
        }
        else
        {
            if (options.IsSelf)
            {
                styleSrcAttr
                    .Self();
            }

            if (options.IsUnsafeInline)
            {
                styleSrcAttr
                    .UnsafeInline();
            }

            if (options.IsUnsafeHashes)
            {
                styleSrcAttr
                    .UnsafeHashes();
            }

            if (options.Sources.Length > 0)
            {
                styleSrcAttr
                    .Sources(options.Sources);
            }

            if (options.ReportSample)
            {
                styleSrcAttr
                    .ReportSample();
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspObject(this CspBuilder builder, CspOptions.CspDirectiveOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        var objectSrc = builder
            .ObjectSrc();

        if (options.IsNone)
        {
            objectSrc
                .None();
        }
        else
        {
            if (options.IsSelf)
            {
                objectSrc
                    .Self();
            }

            if (options.Sources.Length > 0)
            {
                objectSrc
                    .Sources(options.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspImage(this CspBuilder builder, CspOptions.CspDirectiveOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        var objectSrc = builder
            .ImageSrc();

        if (options.IsNone)
        {
            objectSrc
                .None();
        }
        else
        {
            if (options.IsSelf)
            {
                objectSrc
                    .Self();
            }

            if (options.Sources.Length > 0)
            {
                objectSrc
                    .Sources(options.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspMedia(this CspBuilder builder, CspOptions.CspDirectiveOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        var objectSrc = builder
            .MediaSrc();

        if (options.IsNone)
        {
            objectSrc
                .None();
        }
        else
        {
            if (options.IsSelf)
            {
                objectSrc
                    .Self();
            }

            if (options.Sources.Length > 0)
            {
                objectSrc
                    .Sources(options.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspFrame(this CspBuilder builder, CspOptions.CspDirectiveOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        var objectSrc = builder
            .FramesSrc();

        if (options.IsNone)
        {
            objectSrc
                .None();
        }
        else
        {
            if (options.IsSelf)
            {
                objectSrc
                    .Self();
            }

            if (options.Sources.Length > 0)
            {
                objectSrc
                    .Sources(options.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspFencedFrame(this CspBuilder builder, CspOptions.CspDirectiveOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        var objectSrc = builder
            .FencedFramesSrc();

        if (options.IsNone)
        {
            objectSrc
                .None();
        }
        else
        {
            if (options.IsSelf)
            {
                objectSrc
                    .Self();
            }

            if (options.Sources.Length > 0)
            {
                objectSrc
                    .Sources(options.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspFrameAncestor(this CspBuilder builder, CspOptions.CspDirectiveOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        var objectSrc = builder
            .FrameAncestorsSrc();

        if (options.IsNone)
        {
            objectSrc
                .None();
        }
        else
        {
            if (options.IsSelf)
            {
                objectSrc
                    .Self();
            }

            if (options.Sources.Length > 0)
            {
                objectSrc
                    .Sources(options.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspFont(this CspBuilder builder, CspOptions.CspDirectiveOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        var objectSrc = builder
            .FontSrc();

        if (options.IsNone)
        {
            objectSrc
                .None();
        }
        else
        {
            if (options.IsSelf)
            {
                objectSrc
                    .Self();
            }

            if (options.Sources.Length > 0)
            {
                objectSrc
                    .Sources(options.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspConnection(this CspBuilder builder, CspOptions.CspDirectiveOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        var objectSrc = builder
            .ConnectionSrc();

        if (options.IsNone)
        {
            objectSrc
                .None();
        }
        else
        {
            if (options.IsSelf)
            {
                objectSrc
                    .Self();
            }

            if (options.Sources.Length > 0)
            {
                objectSrc
                    .Sources(options.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspBaseUri(this CspBuilder builder, CspOptions.CspDirectiveOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        var objectSrc = builder
            .BaseUri();

        if (options.IsNone)
        {
            objectSrc
                .None();
        }
        else
        {
            if (options.IsSelf)
            {
                objectSrc
                    .Self();
            }

            if (options.Sources.Length > 0)
            {
                objectSrc
                    .Sources(options.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspChildren(this CspBuilder builder, CspOptions.CspDirectiveOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        var objectSrc = builder
            .ChildrenSrc();

        if (options.IsNone)
        {
            objectSrc
                .None();
        }
        else
        {
            if (options.IsSelf)
            {
                objectSrc
                    .Self();
            }

            if (options.Sources.Length > 0)
            {
                objectSrc
                    .Sources(options.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspForm(this CspBuilder builder, CspOptions.CspDirectiveOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        var objectSrc = builder
            .FormSrc();

        if (options.IsNone)
        {
            objectSrc
                .None();
        }
        else
        {
            if (options.IsSelf)
            {
                objectSrc
                    .Self();
            }

            if (options.Sources.Length > 0)
            {
                objectSrc
                    .Sources(options.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspManifest(this CspBuilder builder, CspOptions.CspDirectiveOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        var objectSrc = builder
            .ManifestSrc();

        if (options.IsNone)
        {
            objectSrc
                .None();
        }
        else
        {
            if (options.IsSelf)
            {
                objectSrc
                    .Self();
            }

            if (options.Sources.Length > 0)
            {
                objectSrc
                    .Sources(options.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspWorker(this CspBuilder builder, CspOptions.CspDirectiveOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        var objectSrc = builder
            .WorkerSrc();

        if (options.IsNone)
        {
            objectSrc
                .None();
        }
        else
        {
            if (options.IsSelf)
            {
                objectSrc
                    .Self();
            }

            if (options.Sources.Length > 0)
            {
                objectSrc
                    .Sources(options.Sources);
            }
        }

        return builder;
    }

    internal static CspBuilder UseTrustedTypes(this CspBuilder builder, CspOptions.CspDirectiveTrustedTypesOptions? options)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        var trustedTypes = builder
            .TrustedTypes();

        if (options.IsNone)
        {
            trustedTypes
                .None();
        }
        else
        {
            if (options.AllowDuplicates)
            {
                trustedTypes
                    .AllowDuplicates();
            }

            if (options.Policies.Length > 0)
            {
                trustedTypes
                    .TrustedTypes(options.Policies);
            }
        }

        return builder;
    }

    internal static CspBuilder UseCspSandbox(this CspBuilder builder, CspOptions.CspDirectiveSandboxOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        var sandbox = builder
            .Sandbox();

        if (options.AllowDownloads)
        {
            sandbox
                .AllowDownloads();
        }

        if (options.AllowForms)
        {
            sandbox
                .AllowForms();
        }

        if (options.AllowModals)
        {
            sandbox
                .AllowModals();
        }

        if (options.AllowOrientationLock)
        {
            sandbox
                .AllowOrientationLock();
        }

        if (options.AllowPointerLock)
        {
            sandbox
                .AllowPointerLock();
        }

        if (options.AllowPopups)
        {
            sandbox
                .AllowPopups();
        }

        if (options.AllowPopupsToEscapeSandbox)
        {
            sandbox
                .AllowPopupsToEscapeSandbox();
        }

        if (options.AllowPresentation)
        {
            sandbox
                .AllowPresentation();
        }

        if (options.AllowSameOrigin)
        {
            sandbox
                .AllowSameOrigin();
        }

        if (options.AllowScripts)
        {
            sandbox
                .AllowScripts();
        }

        if (options.AllowStorageAccessByUserActivation)
        {
            sandbox
                .AllowStorageAccessByUserActivation();
        }

        if (options.AllowTopNavigation)
        {
            sandbox
                .AllowTopNavigation();
        }

        if (options.AllowTopNavigationByUserActivation)
        {
            sandbox
                .AllowTopNavigationByUserActivation();
        }

        if (options.AllowTopNavigationToCustomProtocols)
        {
            sandbox
                .AllowTopNavigationToCustomProtocols();
        }

        return builder;
    }

    internal static CspBuilder UseCspRequireTrustedTypesFor(this CspBuilder builder, CspOptions.CspDirectiveScriptsOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        var requireTrustedTypesFor = builder
            .RequireTrustedTypesFor();

        if (options.RequireTrustedTypes)
        {
            requireTrustedTypesFor
                .Script();
        }

        return builder;
    }

    internal static CspBuilder UseCspRequireSriFor(this CspBuilder builder, CspOptions.CspDirectiveScriptsOptions? scriptOptions = null, CspOptions.CspDirectiveStylesOptions? styleOptions = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (scriptOptions == null && styleOptions == null)
        {
            return builder;
        }

        var requireSriFor = builder
            .RequireSriFor();

        if (scriptOptions is { RequireSri: true })
        {
            requireSriFor
                .Script();
        }

        if (styleOptions is { RequireSri: true })
        {
            requireSriFor
                .Script();
        }

        return builder;
    }
}