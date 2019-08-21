using System;
using System.IO;

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
        /// <returns>The content as string.</returns>
        public static string ReadAll(this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            using (var streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}