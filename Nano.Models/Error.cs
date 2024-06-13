using System;
using System.Linq;
using Nano.Models.Exceptions;

namespace Nano.Models;

/// <summary>
/// Error.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary.
    /// </summary>
    public string Summary { get; set; }

    /// <summary>
    /// Exceptions.
    /// </summary>
    public string[] Exceptions { get; set; } = [];

    /// <summary>
    /// Is Translated.
    /// </summary>
    public bool IsTranslated { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public Error()
    {
        this.Summary = "Internal Server Error";
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The message.</param>
    public Error(string message)
        : this()
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        this.Exceptions =
        [
            message
        ];
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="exception">The <see cref="Exception"/>.</param>
    public Error(Exception exception)
        : this()
    {
        if (exception == null)
            throw new ArgumentNullException(nameof(exception));

        if (exception is BadRequestException)
        {
            this.Summary = "Bad Request";
        }

        this.Exceptions =
        [
            exception.Message
        ];

        switch (exception)
        {
            case AggregateException aggregateException:
            {
                if (aggregateException.InnerException != null)
                {
                    switch (aggregateException.InnerException)
                    {
                        case BadRequestException:
                            this.Summary = "Bad Request";
                            this.Exceptions = aggregateException.InnerExceptions
                                .Where(x => x is BadRequestException)
                                .Select(x => x.Message)
                                .ToArray();

                            this.IsTranslated = true;

                            break;

                        case TranslationException:
                            this.Exceptions = aggregateException.InnerExceptions
                                .Where(x => x is TranslationException)
                                .Select(x => x.Message)
                                .ToArray();

                            this.IsTranslated = true;

                            break;
                    }
                }

                break;
            }

            case TranslationException:
                this.IsTranslated = true;

                break;
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var exceptionsString = this.Exceptions
            .Aggregate($"Messages:{Environment.NewLine}", (current, exception) => current + exception + Environment.NewLine);

        return $"{this.Summary}{Environment.NewLine}Messages:{Environment.NewLine}{exceptionsString}";
    }
}