using System;
using Microsoft.AspNetCore.Mvc;

namespace Nano.App.Exceptions;

/// <summary>
/// Problem Details Exception.
/// </summary>
public class ProblemDetailsException : Exception
{
    /// <summary>
    /// Problem Details
    /// </summary>
    public ProblemDetails ProblemDetails { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="problemDetails">The <see cref="ProblemDetails"/>>.</param>
    public ProblemDetailsException(ProblemDetails problemDetails)
    {
        ProblemDetails = problemDetails ?? throw new ArgumentNullException(nameof(problemDetails));
    }
}