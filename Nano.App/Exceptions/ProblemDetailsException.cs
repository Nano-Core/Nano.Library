using Microsoft.AspNetCore.Mvc;
using System;

namespace Nano.App.Exceptions;

/// <summary>
/// Represents an exception that is thrown for identity-related errors.
/// </summary>
public class ProblemDetailsException : Exception
{
    /// <summary>
    /// The problem details object that describes the error.
    /// </summary>
    public ProblemDetails ProblemDetails { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProblemDetailsException"/> class with a specified error message.
    /// </summary>
    /// <param name="problemDetails">The <see cref="Microsoft.AspNetCore.Mvc.ProblemDetails"/> that describes the error.</param>
    public ProblemDetailsException(ProblemDetails problemDetails)
        : base(GetDetailOrThrow(problemDetails))
    {
        this.ProblemDetails = problemDetails;
    }

    private static string? GetDetailOrThrow(ProblemDetails problemDetails)
    {
        ArgumentNullException.ThrowIfNull(problemDetails);

        return problemDetails.Detail;
    }
}