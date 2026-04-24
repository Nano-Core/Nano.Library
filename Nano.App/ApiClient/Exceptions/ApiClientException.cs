using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Nano.App.ApiClient.Exceptions;

/// <summary>
/// Exception thrown by an API client when an HTTP request results in a non-success status code and no valid <see cref="ProblemDetails"/>
/// payload is available.
/// </summary>
public class ApiClientException : Exception
{
    /// <summary>
    /// Gets the HTTP status code returned by the upstream service.
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiClientException"/> class with the raw response content as the error message.
    /// </summary>
    /// <param name="message">The raw response content returned from the upstream service. This may be empty or non-structured if the response did not contain a valid error payload.</param>
    /// <param name="statusCode">The HTTP status code returned by the upstream service.</param>
    public ApiClientException(string message, HttpStatusCode statusCode)
        : base(message)
    {
        this.StatusCode = statusCode;
    }
}