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
            if (formItem.Type == typeof(IFormFile))
            {
                if (formItem.Value is not IFormFile value)
                    return;

                var bytes = await value
                    .OpenReadStream()
                    .ReadAllBytesAsync(cancellationToken);

                var fileContent = new ByteArrayContent(bytes);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(HttpContentType.FORM);

                formContent
                    .Add(fileContent, formItem.Name, value.FileName);
            }
            else if (formItem.Type == typeof(Stream))
            {
                if (formItem.Value is not FileStream value)
                    return;

                var bytes = await value
                    .ReadAllBytesAsync(cancellationToken);

                var fileContent = new ByteArrayContent(bytes);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(HttpContentType.FORM);

                formContent
                    .Add(fileContent, formItem.Name);
            }
            else if (formItem.Type == typeof(FileInfo))
            {
                if (formItem.Value is not FileInfo value)
                    return;

                var filename = value.FullName;

                if (!File.Exists(filename))
                    throw new FileNotFoundException($"File: '{filename}' not found.");

                var bytes = await File.ReadAllBytesAsync(filename, cancellationToken);
                var fileContent = new ByteArrayContent(bytes);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(HttpContentType.FORM);

                formContent
                    .Add(fileContent, formItem.Name, Path.GetFileName(filename));
            }
            else
            {
                var isSimple = formItem.Type
                    .IsSimple();

                var value = isSimple
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