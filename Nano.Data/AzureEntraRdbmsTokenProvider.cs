using Azure.Core;
using Azure.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.Data;

/// <summary>
/// Provides a shared Microsoft Entra (Workload Identity) token provider for Azure-hosted OSS RDBMS servers (MySQL, PostgreSQL).
/// </summary>
public static class AzureEntraRdbmsTokenProvider
{
    private const string DEFAULT_URL = "https://ossrdbms-aad.database.windows.net/.default";

    private static readonly WorkloadIdentityCredential credential = new();

    /// <summary>
    /// Fetches a fresh Microsoft Entra access token for authenticating against an Azure OSS RDBMS server.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
    /// <returns>The access token string.</returns>
    public static async ValueTask<string> GetTokenAsync(CancellationToken cancellationToken)
    {
        var request = new TokenRequestContext([DEFAULT_URL]);

        var token = await credential
            .GetTokenAsync(request, cancellationToken);

        return token.Token;
    }
}