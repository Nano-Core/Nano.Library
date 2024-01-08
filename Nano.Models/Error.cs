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
    /// Message.
    /// </summary>
    public string Summary { get; set; }

    /// <summary>
    /// Description.
    /// </summary>
    public string[] Exceptions { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Is Translated.
    /// </summary>
    public bool IsTranslated { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public Error()
    {

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

        this.Summary = exception is BadRequestException
            ? "Bad Request"
            : "Internal Server Error";

        this.Exceptions = new[]
        {
            exception.Message
        };

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