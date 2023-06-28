using System;
using System.Collections;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nano.Models.Extensions;
using Nano.Web.Api.Requests.Types;
using Nano.Web.Api.Responses;
using Nano.Web.Const;

namespace Nano.Web.Api.Extensions;

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

        if (formItem.Value is IEnumerable array)
        {
            var isAdded = false;
            foreach (var element in array)
            {
                switch (element)
                {
                    case IFormFile formFile:
                        await formContent
                            .Add(formFile, formItem.Name, cancellationToken);

                        isAdded = true;
                        break;

                    case FileInfo fileInfo:
                        await formContent
                            .Add(fileInfo, formItem.Name, cancellationToken);

                        isAdded = true;
                        break;

                    case FileStream fileStream:
                        await formContent
                            .Add(fileStream, formItem.Name, cancellationToken);

                        isAdded = true;
                        break;

                    case Stream stream:
                        await formContent
                            .Add(stream, formItem.Name, cancellationToken);

                        isAdded = true;
                        break;

                    case NamedStream namedStream:
                        await formContent
                            .Add(namedStream, formItem.Name, cancellationToken);

                        isAdded = true;
                        break;
                }
            }

            if (!isAdded)
            {
                await formContent
                    .Add(formItem.Value, formItem.Name);
            }
        }
        else
        {
            switch (formItem.Value)
            {
                case IFormFile formFile:
                    await formContent
                        .Add(formFile, formItem.Name, cancellationToken);
                    break;

                case FileInfo fileInfo:
                    await formContent
                        .Add(fileInfo, formItem.Name, cancellationToken);
                    break;

                case FileStream fileStream:
                    await formContent
                        .Add(fileStream, formItem.Name, cancellationToken);
                    break;

                case Stream stream:
                    await formContent
                        .Add(stream, formItem.Name, cancellationToken);
                    break;

                case NamedStream namedStream:
                    await formContent
                        .Add(namedStream, formItem.Name, cancellationToken);
                    break;

                default:
                    await formContent
                        .Add(formItem.Value, formItem.Name);
                    break;
            }
        }
    }

    private static async Task Add(this MultipartFormDataContent formContent, FileInfo fileInfo, string name, CancellationToken cancellationToken = default)
    {
        if (formContent == null)
            throw new ArgumentNullException(nameof(formContent));

        if (fileInfo == null)
            throw new ArgumentNullException(nameof(fileInfo));

        if (name == null)
            throw new ArgumentNullException(nameof(name));

        if (!File.Exists(fileInfo.FullName))
        {
            throw new FileNotFoundException($"File: '{fileInfo.FullName}' not found.");
        }

        var bytes = await File.ReadAllBytesAsync(fileInfo.FullName, cancellationToken);
        var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(HttpContentType.FORM);

        formContent
            .Add(fileContent, name, fileInfo.Name);
    }
    private static async Task Add(this MultipartFormDataContent formContent, IFormFile formFile, string name, CancellationToken cancellationToken = default)
    {
        if (formContent == null)
            throw new ArgumentNullException(nameof(formContent));

        if (formFile == null)
            throw new ArgumentNullException(nameof(formFile));

        if (name == null)
            throw new ArgumentNullException(nameof(name));

        var stream = formFile
            .OpenReadStream();

        await using (stream)
        {
            var bytes = await stream
                .ReadAllBytesAsync(cancellationToken);

            var fileContent = new ByteArrayContent(bytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(HttpContentType.FORM);

            formContent
                .Add(fileContent, name, formFile.FileName);
        }
    }
    private static async Task Add(this MultipartFormDataContent formContent, Stream stream, string name, CancellationToken cancellationToken = default)
    {
        if (formContent == null)
            throw new ArgumentNullException(nameof(formContent));

        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        if (name == null)
            throw new ArgumentNullException(nameof(name));

        var bytes = await stream
            .ReadAllBytesAsync(cancellationToken);

        var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(HttpContentType.FORM);

        formContent
            .Add(fileContent, name, "Stream.name");
    }
    private static async Task Add(this MultipartFormDataContent formContent, FileStream fileStream, string name, CancellationToken cancellationToken = default)
    {
        if (formContent == null)
            throw new ArgumentNullException(nameof(formContent));

        if (fileStream == null)
            throw new ArgumentNullException(nameof(fileStream));

        if (name == null)
            throw new ArgumentNullException(nameof(name));

        var bytes = await fileStream
            .ReadAllBytesAsync(cancellationToken);

        var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(HttpContentType.FORM);

        formContent
            .Add(fileContent, name, fileStream.Name);
    }
    private static async Task Add(this MultipartFormDataContent formContent, NamedStream namedStream, string name, CancellationToken cancellationToken = default)
    {
        if (formContent == null)
            throw new ArgumentNullException(nameof(formContent));

        if (namedStream == null)
            throw new ArgumentNullException(nameof(namedStream));

        if (name == null)
            throw new ArgumentNullException(nameof(name));

        await using (namedStream.Stream)
        {
            var bytes = await namedStream.Stream
                .ReadAllBytesAsync(cancellationToken);

            var fileContent = new ByteArrayContent(bytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(HttpContentType.FORM);

            formContent
                .Add(fileContent, name, namedStream.Name);
        }
    }
    private static async Task Add(this MultipartFormDataContent formContent, object value, string name)
    {
        if (formContent == null)
            throw new ArgumentNullException(nameof(formContent));

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        if (name == null)
            throw new ArgumentNullException(nameof(name));

        var isSimple = value.GetType()
            .IsSimple();

        var content = !isSimple
            ? JsonSerializer.Serialize(value, Globals.jsonSerializerSettings)
            : value.ToString() ?? string.Empty;

        formContent
            .Add(new StringContent(content), name);

        await Task.CompletedTask;
    }
}