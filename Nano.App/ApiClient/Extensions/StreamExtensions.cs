using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.App.ApiClient.Extensions;

internal static class StreamExtensions
{
    internal static async Task<byte[]> ReadAllBytesAsync(this Stream stream, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        await using var memoryStream = new MemoryStream();

        await stream
            .CopyToAsync(memoryStream, cancellationToken);

        return memoryStream
            .ToArray();
    }
}