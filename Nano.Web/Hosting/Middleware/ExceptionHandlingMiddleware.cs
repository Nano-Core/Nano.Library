using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Nano.Models.Exceptions;
using Nano.Security.Exceptions;
using Nano.Web.Const;
using Nano.Web.Extensions.Const;
using Nano.Web.Hosting.Serialization;
using Nano.Web.Models;

namespace Nano.Web.Hosting.Middleware;

/// <inheritdoc />
public class ExceptionHandlingMiddleware : IMiddleware
{
    private const string MESSAGE_TEMPLATE = "{protocol} {method} {pathAndqueryString} {statusCode} in {elapsed:0.0000} ms. (Id={id})";

    /// <summary>
    /// Logger.
    /// </summary>
    protected virtual ILogger Logger { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="logger">the <see cref="ILogger"/></param>
    public ExceptionHandlingMiddleware(ILogger logger)
    {
        this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        if (httpContext == null)
            throw new ArgumentNullException(nameof(httpContext));

        if (next == null)
            throw new ArgumentNullException(nameof(next));

        var timestamp = Stopwatch.GetTimestamp();

        var request = httpContext.Request;
        var response = httpContext.Response;

        var logLevel = response.StatusCode is >= 500 and <= 599
            ? LogLevel.Error
            : LogLevel.Information;

        Exception exception = default;
        try
        {
            await next(httpContext);
        }
        catch (UnauthorizedException)
        {
            response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
        catch (PermissionDeniedException)
        {
            response.StatusCode = (int)HttpStatusCode.Forbidden;
        }
        catch (TaskCanceledException)
        {
            response.StatusCode = (int)HttpStatusCode.NoContent;
        }
        catch (OperationCanceledException)
        {
            response.StatusCode = (int)HttpStatusCode.NoContent;
        }
        catch (Exception ex)
        {
            if (response.HasStarted)
            {
                response.Clear();
            }

            response.StatusCode = ex is BadRequestException || ex.InnerException is BadRequestException
                ? (int)HttpStatusCode.BadRequest
                : (int)HttpStatusCode.InternalServerError;

            exception = ex.GetBaseException();

            var error = new Error(exception);

            logLevel = error.IsTranslated
                ? LogLevel.Information
                : LogLevel.Error;

            var acceptHheader = request.Headers["Accept"];
            var contentTypeHeader = request.Headers["Content-Type"];
            var queryString = request.QueryString.HasValue
                ? request.QueryString.Value ?? string.Empty
                : string.Empty;

            var result = acceptHheader.Any()
                ? acceptHheader.Contains(HttpContentType.JSON)
                    ? JsonSerializer.Serialize(error, Globals.jsonSerializerSettings)
                    : acceptHheader.Contains(HttpContentType.XML)
                        ? XmlConvert.SerializeObject(error)
                        : acceptHheader.Contains(HttpContentType.FORM) || acceptHheader.Contains(HttpContentType.FORM_ENCODED)
                            ? JsonSerializer.Serialize(error, Globals.jsonSerializerSettings)
                            : $"{error.Summary}: {error.Exceptions.FirstOrDefault()}"
                : contentTypeHeader.Any()
                    ? contentTypeHeader.Contains(HttpContentType.JSON)
                        ? JsonSerializer.Serialize(error, Globals.jsonSerializerSettings)
                        : contentTypeHeader.Contains(HttpContentType.XML)
                            ? XmlConvert.SerializeObject(error)
                            : acceptHheader.Contains(HttpContentType.FORM) || acceptHheader.Contains(HttpContentType.FORM_ENCODED)
                                ? JsonSerializer.Serialize(error, Globals.jsonSerializerSettings)
                                : $"{error.Summary}: {error.Exceptions.FirstOrDefault()}"
                    : queryString.Contains($"format={HttpContentType.JSON}")
                        ? JsonSerializer.Serialize(error, Globals.jsonSerializerSettings)
                        : queryString.Contains($"format={HttpContentType.XML}")
                            ? XmlConvert.SerializeObject(error)
                            : $"{error.Summary}: {error.Exceptions.FirstOrDefault()}";

            await response
                .WriteAsync(result);
        }
        finally
        {
            var protocol = request.IsHttps
                ? request.Protocol.Replace("HTTP", "HTTPS")
                : request.Protocol;

            var method = request.Method;
            var path = request.Path.Value;
            var queryString = request.QueryString.HasValue ? $"{request.QueryString.Value}" : null;

            var success = request.Query
                .TryGetValue("access_token", out var accessToken);

            if (success)
            {
                queryString = queryString?
                    .Replace(accessToken, "<<secret>>");
            }

            var pathAndqueryString = $"{path}{queryString}";
            var elapsed = (Stopwatch.GetTimestamp() - timestamp) * 1000D / Stopwatch.Frequency;
            var id = httpContext.TraceIdentifier;

            var isHealthCheck = logLevel == LogLevel.Information && path == HealthzCheckUris.Path;

            if (!isHealthCheck)
            {
                this.Logger
                    .Log(logLevel, exception, MESSAGE_TEMPLATE, protocol, method, pathAndqueryString, response.StatusCode, elapsed, id);
            }
        }
    }
}