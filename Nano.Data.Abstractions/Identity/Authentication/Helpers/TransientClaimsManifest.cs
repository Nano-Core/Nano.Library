using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Nano.Data.Abstractions.Identity.Consts;

namespace Nano.Data.Abstractions.Identity.Authentication.Helpers;

/// <summary>
/// Builds and reads the manifest claim recording exactly which non-persisted "transient" roles/claims
/// were asserted at login, so a refresh can recover and carry forward that same set instead of trusting
/// the refresh caller to resupply it - a refresh can then never grant more than the original login did.
/// Internal to Nano.Library's own login/refresh implementations, not a public extension point.
/// </summary>
public static class TransientClaimsManifest
{
    /// <summary>
    /// Builds the manifest claim for the given transient roles/claims, to embed alongside the real
    /// claims on every issued token - including on refresh, so the manifest survives the whole refresh
    /// chain. The value is base64-encoded JSON: opaque by convention, not meant to be read directly.
    /// </summary>
    /// <param name="transientRoles">The transient roles asserted at login.</param>
    /// <param name="transientClaims">The transient claims asserted at login.</param>
    /// <returns>The manifest claim.</returns>
    public static Claim Build(IEnumerable<string> transientRoles, IEnumerable<KeyValuePair<string, string>> transientClaims)
    {
        ArgumentNullException.ThrowIfNull(transientRoles);
        ArgumentNullException.ThrowIfNull(transientClaims);

        var entries = transientRoles
            .Select(x => new[] { ClaimTypes.Role, x })
            .Concat(transientClaims
                .Select(x => new[] { x.Key, x.Value }));

        var json = JsonSerializer.Serialize(entries);
        var value = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

        return new Claim(ClaimTypesExtended.TransientClaimsManifest, value);
    }

    /// <summary>
    /// Reads the transient roles and claims back out of a manifest claim previously produced by
    /// <see cref="Build"/>, splitting role entries back out from plain claims. Returns two empty
    /// collections if no manifest claim is present.
    /// </summary>
    /// <param name="claims">The claims of the token being refreshed.</param>
    /// <returns>The recovered transient roles and claims.</returns>
    public static (IEnumerable<string> TransientRoles, IEnumerable<KeyValuePair<string, string>> TransientClaims) Parse(IEnumerable<Claim> claims)
    {
        ArgumentNullException.ThrowIfNull(claims);

        var value = claims
            .Where(x => x.Type == ClaimTypesExtended.TransientClaimsManifest)
            .Select(x => x.Value)
            .FirstOrDefault();

        if (string.IsNullOrEmpty(value))
        {
            return ([], []);
        }

        var json = Encoding.UTF8.GetString(Convert.FromBase64String(value));
        var entries = JsonSerializer.Deserialize<string[][]>(json) ?? [];

        var transientRoles = entries
            .Where(x => x[0] == ClaimTypes.Role)
            .Select(x => x[1]);

        var transientClaims = entries
            .Where(x => x[0] != ClaimTypes.Role)
            .Select(x => new KeyValuePair<string, string>(x[0], x[1]));

        return (transientRoles, transientClaims);
    }
}
