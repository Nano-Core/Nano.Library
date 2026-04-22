using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nano.App.Api.Config;
using Nano.App.Api.Mvc.HealthChecks.Const;
using Nano.App.Consts;
using Nano.App.Exceptions;
using Nano.App.Extensions;
using Nano.Data.Abstractions.Exceptions;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Vivet.AspNetCore.RequestVirusScan.Exceptions;

namespace Nano.App.Api.Mvc.Middleware;

/// <summary>
/// Middleware to handle exceptions globally, log them, and return structured <see cref="ProblemDetails"/> responses.
/// Supports various custom exceptions, translation, and UX-specific error handling.
/// </summary>
/// <param name="logger">The <see cref="ILogger{T}"/> used for logging.</param>
/// <param name="apiOptions">The <see cref="IOptionsMonitor{ApiOptions}"/> containing API configuration.</param>
public sealed class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, IOptionsMonitor<ApiOptions> apiOptions)
    : IMiddleware
{
    private const string MESSAGE_TEMPLATE = "{protocol} {method} {path}{queryString} {statusCode} in {elapsed:0.0000} ms. (Id={id})";

    private ILogger<ExceptionHandlingMiddleware> Logger { get; } = logger ?? throw new ArgumentNullException(nameof(logger));
    private IOptionsMonitor<ApiOptions> ApiOptions { get; } = apiOptions ?? throw new ArgumentNullException(nameof(apiOptions));

    /// <summary>
    /// Invokes the middleware to handle exceptions, log request information, and write structured problem details responses.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/> for the current request.</param>
    /// <param name="next">The next <see cref="RequestDelegate"/> in the pipeline.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(next);

        var timestamp = Stopwatch.GetTimestamp();

        Exception? exception = null;
        Exception[] exceptions = [];

        var problemDetails = new ProblemDetails();

        try
        {
            await next(httpContext);
        }
        catch (NotFoundException)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }
        catch (VirusScanException ex)
        {
            exception = ex;

            problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc9110-15.6.1#name-422-unprocessable-content";
            problemDetails.Status = (int)HttpStatusCode.UnprocessableEntity;
            problemDetails.Title = "Unprocessable Entity";
        }
        catch (UnauthorizedException ex)
        {
            exception = ex;

            problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc9110-15.6.1#name-401-unauthorized";
            problemDetails.Status = (int)HttpStatusCode.Unauthorized;
            problemDetails.Title = "Unauthorized";
        }
        catch (PermissionDeniedException ex)
        {
            exception = ex;

            problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc9110-15.6.1#name-403-forbidden";
            problemDetails.Status = (int)HttpStatusCode.Forbidden;
            problemDetails.Title = "Forbidden";
        }
        catch (IdentityException ex)
        {
            exception = ex;

            problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc9110-15.6.1#name-400-bad-request";
            problemDetails.Status = (int)HttpStatusCode.BadRequest;
            problemDetails.Title = "Bad Request";
        }
        catch (BadRequestException ex)
        {
            exception = ex;

            problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc9110-15.6.1#name-400-bad-request";
            problemDetails.Status = (int)HttpStatusCode.BadRequest;
            problemDetails.Title = "Bad Request";

            if (ex.IsCoded)
            {
                problemDetails.Extensions
                    .Add(ProblemDetailsExtensionKeys.IS_CODED, true);
            }

            if (ex.IsTranslated)
            {
                problemDetails.Extensions
                    .Add(ProblemDetailsExtensionKeys.IS_TRANSLATED, true);
            }
        }
        catch (OperationCanceledException ex)
        {
            exception = ex;

            problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc9110-15.6.1#section-15.5.9";
            problemDetails.Status = (int)HttpStatusCode.RequestTimeout;
            problemDetails.Title = "Request Timeout";
        }
        catch (ProblemDetailsException ex)
        {
            exception = ex;

            problemDetails = ex.ProblemDetails;
        }
        catch (UniqueConstraintViolationException ex)
        {
            exception = ex;

            problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc9110-15.6.1#name-409-conflict";
            problemDetails.Status = (int)HttpStatusCode.Conflict;
            problemDetails.Title = "Conflict";
        }
        catch (AggregateException ex)
        {
            exception = ex;
            exceptions = ex.InnerExceptions.ToArray();

            problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc9110-15.6.1#name-500-internal-server-error";
            problemDetails.Status = (int)HttpStatusCode.InternalServerError;
            problemDetails.Title = "Internal Server Error";

            if (ex.InnerException is BadRequestException or IdentityException)
            {
                problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc9110-15.6.1#name-400-bad-request";
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = "Bad Request";
            }
        }
        catch (Exception ex)
        {
            exception = ex;

            problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc9110-15.6.1#name-500-internal-server-error";
            problemDetails.Status = (int)HttpStatusCode.InternalServerError;
            problemDetails.Title = "Internal Server Error";
        }
        finally
        {
            if (exception != null)
            {
                if (!httpContext.Response.HasStarted)
                {
                    httpContext.Response
                        .Clear();

                    httpContext.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

                    if (httpContext.Response.StatusCode != (int)HttpStatusCode.InternalServerError || this.ApiOptions.CurrentValue.ErrorHandling.ExposeErrors)
                    {
                        problemDetails.Detail = exception.Message;
                        problemDetails.Extensions
                            .Add(ProblemDetailsExtensionKeys.ERRORS, exceptions.Select(x => x.Message).ToArray());
                    }
                    else
                    {
                        problemDetails.Detail = null;
                        problemDetails.Extensions
                            .Remove(ProblemDetailsExtensionKeys.ERRORS);
                    }

                    await httpContext.Response
                        .WriteAsJsonAsync(problemDetails);
                }
                else
                {
                    this.Logger
                        .LogError(exception, "Unhandled exception after response started");
                }
            }

            var logLevel = httpContext.Response.StatusCode == (int)HttpStatusCode.InternalServerError
                ? LogLevel.Error
                : LogLevel.Information;

            this.LogRequest(httpContext.Request, logLevel, httpContext.Response.StatusCode, timestamp, exception);
        }
    }


    private void LogRequest(HttpRequest httpRequest, LogLevel logLevel, int statusCode, long timestamp, Exception? exception = null)
    {
        ArgumentNullException.ThrowIfNull(httpRequest);

        var protocol = httpRequest.IsHttps
            ? httpRequest.Protocol.Replace("HTTP", "HTTPS")
            : httpRequest.Protocol;

        var method = httpRequest.Method;
        var path = httpRequest.Path.Value;
        var queryString = httpRequest.QueryString.HasValue ? $"{httpRequest.QueryString.Value}" : null;

        var success = httpRequest.Query
            .TryGetValue("access_token", out var accessToken);

        if (success)
        {
            queryString = queryString?
                .Replace(accessToken.ToString(), "<<secret>>");
        }

        var elapsed = (Stopwatch.GetTimestamp() - timestamp) * 1000D / Stopwatch.Frequency;

        var id = httpRequest
            .GetRequestId();

        var isHealthCheck = logLevel == LogLevel.Information && path == HealthzCheckUris.Path;

        if (!isHealthCheck)
        {
            this.Logger
                .Log(logLevel, exception, MESSAGE_TEMPLATE, protocol, method, path, queryString, statusCode, elapsed, id);
        }
    }
}