using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.Models.Extensions
{
    /// <summary>
    /// Stream Extensions.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Reads all bytes in the <see cref="Stream"/> and returns the content as string.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The content as string.</returns>
        public static async Task<string> ReadAllAsync(this Stream stream, CancellationToken cancellationToken = default)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            using var streamReader = new StreamReader(stream);
            
            return await streamReader
                .ReadToEndAsync();
        }

        /// <summary>
        /// Reads all bytes in the <see cref="Stream"/> and returns the content as byte array.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The content as byte array.</returns>
        public static async Task<byte[]> ReadAllBytesAsync(this Stream stream, CancellationToken cancellationToken = default)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            await using var memoryStream = new MemoryStream();
            {
                await stream
                    .CopyToAsync(memoryStream, cancellationToken);
                
                return memoryStream
                    .ToArray();
            }
        }
    }
}