using System;

namespace Nano.App.Exceptions;

internal class BadRequestException : Exception
{
    internal BadRequestException(string message)
        : base(message)
    {
    }
}