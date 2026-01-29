using System;
using System.Collections;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nano.App.ApiClient.Models;
using Nano.App.ApiClient.Requests.Models;
using Nano.Common.Consts;
using Nano.Common.Extensions;
using Nano.Common.Serialization.Json;
using Newtonsoft.Json;

namespace Nano.App.ApiClient.Extensions;

internal static class MultipartFormDataContentExtensions
{
    internal static async Task AddFormItem(this MultipartFormDataContent formContent, FormItem formItem, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(formContent);
        ArgumentNullException.ThrowIfNull(formItem);

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

    private static async Task Add(this MultipartFormDataContent formContent, FileSystemInfo fileInfo, string name, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(formContent);
        ArgumentNullException.ThrowIfNull(fileInfo);
        ArgumentNullException.ThrowIfNull(name);

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
        ArgumentNullException.ThrowIfNull(formContent);
        ArgumentNullException.ThrowIfNull(formFile);
        ArgumentNullException.ThrowIfNull(name);

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
        ArgumentNullException.ThrowIfNull(formContent);
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(name);

        var bytes = await stream
            .ReadAllBytesAsync(cancellationToken);

        var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(HttpContentType.FORM);

        formContent
            .Add(fileContent, name, "Stream.name");
    }
    private static async Task Add(this MultipartFormDataContent formContent, FileStream fileStream, string name, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(formContent);
        ArgumentNullException.ThrowIfNull(fileStream);
        ArgumentNullException.ThrowIfNull(name);

        var bytes = await fileStream
            .ReadAllBytesAsync(cancellationToken);

        var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(HttpContentType.FORM);

        formContent
            .Add(fileContent, name, fileStream.Name);
    }
    private static async Task Add(this MultipartFormDataContent formContent, NamedStream namedStream, string name, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(formContent);
        ArgumentNullException.ThrowIfNull(namedStream);
        ArgumentNullException.ThrowIfNull(name);

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
    private static Task Add(this MultipartFormDataContent formContent, object? value, string name)
    {
        ArgumentNullException.ThrowIfNull(formContent);
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(name);

        var isSimple = value
            .GetType()
            .IsSimple();

        var content = !isSimple
            ? JsonConvert.SerializeObject(value, SerializerSettings.GetDefault())
            : value.ToString() ?? string.Empty;

        formContent
            .Add(new StringContent(content), name);

        return Task.CompletedTask;
    }
}