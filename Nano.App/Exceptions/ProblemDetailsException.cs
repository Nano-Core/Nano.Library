using System;
using Microsoft.AspNetCore.Mvc;

namespace Nano.App.Exceptions;

internal class ProblemDetailsException : Exception
{
    internal ProblemDetails ProblemDetails { get; set; }

    internal ProblemDetailsException(ProblemDetails problemDetails)
    {
        ProblemDetails = problemDetails ?? throw new ArgumentNullException(nameof(problemDetails));
    }
}