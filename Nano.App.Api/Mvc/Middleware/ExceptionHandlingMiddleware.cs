using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nano.App.Api.Config;
using Nano.App.Api.Exceptions;
using Nano.App.Api.Mvc.HealthChecks.Const;
using Nano.App.Consts;
using Nano.App.Exceptions;
using Nano.Common.Serialization.Json;
using Nano.Data.Abstractions.Annotations;
using Nano.Data.Abstractions.Identity.Exceptions;
using Newtonsoft.Json;
using Vivet.AspNetCore.RequestVirusScan.Exceptions;

namespace Nano.App.Api.Mvc.Middleware;

/// <summary>
/// Middleware to handle exceptions globally, log them, and return structured <see cref="ProblemDetails"/> responses.
/// Supports various custom exceptions, translation, and UX-specific error handling.
/// </summary>
public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private const string MESSAGE_TEMPLATE = "{protocol} {method} {pathAndqueryString} {statusCode} in {elapsed:0.0000} ms. (Id={id})";

    private ILogger Logger { get; }
    private IOptionsMonitor<ApiOptions> ApiOptions { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> used for logging.</param>
    /// <param name="apiOptions">The <see cref="IOptionsMonitor{ApiOptions}"/> containing API configuration.</param>
    public ExceptionHandlingMiddleware(ILogger logger, IOptionsMonitor<ApiOptions> apiOptions)
    {
        this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.ApiOptions = apiOptions ?? throw new ArgumentNullException(nameof(apiOptions));
    }

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

        var request = httpContext.Request;
        var response = httpContext.Response;

        var logLevel = response.StatusCode is >= 500 and <= 599
            ? LogLevel.Error
            : LogLevel.Information;

        Exception? exception = null;
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

            var exceptionMessages = exceptions
                .Select(x => x.Message)
                .ToArray();

            var problemDetails = new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc9110-15.6.1",
                Status = response.StatusCode,
                Title = "Internal Server Error",
                Detail = topException.Message,
                Extensions =
                {
                    { ProblemDetailsExtensions.EXCEPTIONS, exceptionMessages }
                }
            };

            var uxExceptionAttribute = GetUxExceptionAttribute(httpContext, topException);

            if (uxExceptionAttribute == null)
            {
                switch (topException)
                {
                    case BadRequestException:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;

                        problemDetails.Status = response.StatusCode;
                        problemDetails.Title = "Bad Request";

                        problemDetails.Extensions
                            .Add(ProblemDetailsExtensions.IS_TRANSLATED, true);

                        break;

                    case VirusScanException:
                        problemDetails.Title = "Virus Scan Error";

                        break;

                    case CodedException:
                        problemDetails.Title = "Coded Error";

                        problemDetails.Extensions
                            .Add(ProblemDetailsExtensions.IS_CODED, true);

                        break;

                    case IdentityException:
                    case TranslationException:
                        problemDetails.Title = "Translated Error";

                        problemDetails.Extensions
                            .Add(ProblemDetailsExtensions.IS_TRANSLATED, true);

                        break;

                    case ProblemDetailsException problemDetailsException:
                        problemDetails = problemDetailsException.ProblemDetails;

                        break;

                    default:
                        if (!this.ApiOptions.CurrentValue.Hosting.ExposeErrors)
                        {
                            problemDetails.Detail = null;

                            problemDetails.Extensions
                                .Remove(ProblemDetailsExtensions.EXCEPTIONS);
                        }

                        break;
                }
            }
            else
            {
                problemDetails.Detail = uxExceptionAttribute.Message;

                problemDetails.Extensions
                    .Add(ProblemDetailsExtensions.IS_CODED, true);
            }

            logLevel = SetLogLevel(problemDetails);

            var serializerSettings = SerializerSettings.GetDefault();
            var result = JsonConvert.SerializeObject(problemDetails, serializerSettings);

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
                    .Replace(accessToken.ToString(), "<<secret>>");
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


    private static UxExceptionAttribute? GetUxExceptionAttribute(HttpContext httpContext, Exception exception)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

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
    private static LogLevel SetLogLevel(ProblemDetails problemDetails)
    {
        ArgumentNullException.ThrowIfNull(problemDetails);

        problemDetails.Extensions
            .TryGetValue(ProblemDetailsExtensions.IS_TRANSLATED, out var isTranslated);

        problemDetails.Extensions
            .TryGetValue(ProblemDetailsExtensions.IS_CODED, out var isCoded);

        bool.TryParse(isTranslated?.ToString(), out var boolIsTranslated);
        bool.TryParse(isCoded?.ToString(), out var boolIsCoded);

        return boolIsTranslated || boolIsCoded
            ? LogLevel.Information
            : LogLevel.Error;
    }
}