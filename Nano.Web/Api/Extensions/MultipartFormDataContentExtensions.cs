using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nano.Models.Extensions;
using Nano.Web.Api.Requests.Types;
using Nano.Web.Const;
using Newtonsoft.Json;

namespace Nano.Web.Api.Extensions
{
    /// <summary>
    /// Multipart Form Data Content Extensions.
    /// </summary>
    internal static class MultipartFormDataContentExtensions
    {
        /// <summary>
        /// Add For Item.
        /// </summary>
        /// <param name="formContent">The <see cref="MultipartFormDataContent"/>.</param>
        /// <param name="formItem">The <see cref="FormItem"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void.</returns>
        internal static async Task AddFormItem(this MultipartFormDataContent formContent, FormItem formItem, CancellationToken cancellationToken = default)
        {
            if (formContent == null) 
                throw new ArgumentNullException(nameof(formContent));

            if (formItem == null) 
                throw new ArgumentNullException(nameof(formItem));

            if (formItem.Value is IFormFile valueFormFile)
            {
                var stream = valueFormFile
                    .OpenReadStream();

                await using (stream)
                {
                    var bytes = await stream
                        .ReadAllBytesAsync(cancellationToken);

                    var fileContent = new ByteArrayContent(bytes);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(HttpContentType.FORM);

                    formContent
                        .Add(fileContent, formItem.Name, valueFormFile.FileName);
                }
            }
            else if (formItem.Value is FileStream valueFileStream)
            {
                var bytes = await valueFileStream
                    .ReadAllBytesAsync(cancellationToken);

                var fileContent = new ByteArrayContent(bytes);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(HttpContentType.FORM);

                formContent
                    .Add(fileContent, formItem.Name, valueFileStream.Name);
            }
            else if (formItem.Value is FileInfo valueFileInfo)
            {
                var filename = valueFileInfo.FullName;

                if (!File.Exists(filename))
                {
                    throw new FileNotFoundException($"File: '{filename}' not found.");
                }

                var bytes = await File.ReadAllBytesAsync(filename, cancellationToken);
                var fileContent = new ByteArrayContent(bytes);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(HttpContentType.FORM);

                formContent
                    .Add(fileContent, formItem.Name, valueFileInfo.Name);
            }
            else
            {
                var isSimple = formItem.Value.GetType()
                    .IsSimple();

                var value = !isSimple
                    ? JsonConvert.SerializeObject(formItem.Value, new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Error,
                            NullValueHandling = NullValueHandling.Ignore 
                        })
                    : formItem.Value.ToString() ?? string.Empty;

                formContent
                    .Add(new StringContent(value), formItem.Name);
            }
        }
    }
}