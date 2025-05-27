using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Nano.Models;
using Nano.Models.Attributes;
using Nano.Models.Exceptions;
using Nano.Models.Serialization.Json.Const;
using Nano.Security.Exceptions;
using Nano.Web.Extensions.Const;
using Newtonsoft.Json;
using Vivet.AspNetCore.RequestVirusScan.Exceptions;

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
    /// Web Options.
    /// </summary>
    protected virtual WebOptions WebOptions { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="logger">the <see cref="ILogger"/></param>
    /// <param name="webOptions">The <see cref="Nano.Web.WebOptions"/>.</param>
    public ExceptionHandlingMiddleware(ILogger logger, WebOptions webOptions)
    {
        this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.WebOptions = webOptions ?? throw new ArgumentNullException(nameof(webOptions));
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

        Exception exception = null;
        try
        {
            await next(httpContext);
        }
        catch (UnauthorizedException ex)
        {
            exception = ex.GetBaseException();
            response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
        catch (PermissionDeniedException ex)
        {
            exception = ex.GetBaseException();
            response.StatusCode = (int)HttpStatusCode.Forbidden;
        }
        catch (TaskCanceledException ex)
        {
            exception = ex.GetBaseException();
            response.StatusCode = (int)HttpStatusCode.NoContent;
        }
        catch (OperationCanceledException ex)
        {
            exception = ex.GetBaseException();
            response.StatusCode = (int)HttpStatusCode.NoContent;
        }
        catch (Exception ex)
        {
            exception = ex;

            if (response.HasStarted)
            {
                response.Clear();
            }

            response.StatusCode = (int)HttpStatusCode.InternalServerError;

            Exception topException;
            Exception[] exceptions;
            if (ex is AggregateException aggregateException)
            {
                topException = aggregateException.InnerException ?? aggregateException;
                exceptions = aggregateException.InnerExceptions.ToArray();
            }
            else
            {
                topException = ex;
                exceptions = [topException];
            }

            var error = new Error();
            var uxExceptionAttribute = this.GetUxExceptionAttribute(httpContext, topException);

            if (uxExceptionAttribute == null)
            {
                switch (topException)
                {
                    case BadRequestException:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;

                        error.Summary = "Bad Request";
                        error.IsTranslated = true;

                        break;

                    case VirusScanException:
                        error.Summary = "Virus Scan Error";

                        break;

                    case CodedException:
                        error.Summary = "Coded Error";
                        error.IsCoded = true;

                        break;

                    case TranslationException:
                        error.Summary = "Translated Error";
                        error.IsTranslated = true;

                        break;

                    default:
                        var message = "An error occurred.";

                        if (this.WebOptions.Hosting.ExposeErrors)
                        {
                            message = $"{topException.GetType().Name} - {topException.Message}";
                        }
                        
                        error.Summary = "Internal Server Error";
                        error.Exceptions =
                        [
                            message
                        ];

                        break;
                }

                error.Exceptions = exceptions
                    .Select(x => x.Message)
                    .ToArray();
            }
            else
            {
                error.Summary = "Internal Server Error";
                error.Exceptions =
                [
                    uxExceptionAttribute.Message 
                ];
                error.IsCoded = true;
            }

            logLevel = error.IsTranslated
                ? LogLevel.Information
                : LogLevel.Error;

            var serializerSettings = Globals.GetDefaultJsonSerializerSettings();
            var result = JsonConvert.SerializeObject(error, serializerSettings);

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

    private UxExceptionAttribute GetUxExceptionAttribute(HttpContext httpContext, Exception exception)
    {
        if (httpContext == null)
            throw new ArgumentNullException(nameof(httpContext));
        
        var endpoint = httpContext
            .GetEndpoint();

        var actionDescriptor = endpoint?.Metadata
            .GetMetadata<ControllerActionDescriptor>();

        var type = actionDescriptor?.ControllerTypeInfo.BaseType?.GenericTypeArguments
            .FirstOrDefault();

        return type?
            .GetCustomAttributes<UxExceptionAttribute>(true)
            .FirstOrDefault(x =>
                exception.Message
                    .Contains("UX") &&
                exception.Message
                    .Contains(x.Properties
                        .Aggregate(string.Empty, (current, y) => current + $"_{y}")));
    }
}