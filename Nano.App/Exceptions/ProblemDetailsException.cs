using System;
using Microsoft.AspNetCore.Mvc;

namespace Nano.App.Exceptions;

/// <summary>
/// Represents an exception that carries <see cref="Microsoft.AspNetCore.Mvc.ProblemDetails"/> for API error responses.
/// </summary>
public class ProblemDetailsException : Exception
{
    /// <summary>
    /// Gets or sets the <see cref="Microsoft.AspNetCore.Mvc.ProblemDetails"/> associated with the exception.
    /// </summary>
    public ProblemDetails ProblemDetails { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProblemDetailsException"/> class with the specified <see cref="Microsoft.AspNetCore.Mvc.ProblemDetails"/>.
    /// </summary>
    /// <param name="problemDetails">The problem details to attach to this exception.</param>
    public ProblemDetailsException(ProblemDetails problemDetails)
    {
        ProblemDetails = problemDetails ?? throw new ArgumentNullException(nameof(problemDetails));
    }
}