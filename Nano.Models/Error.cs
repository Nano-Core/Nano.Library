using System;
using System.Linq;
using System.Net;
using Nano.Models.Exceptions;

namespace Nano.Models
{
    /// <summary>
    /// Error.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Message.
        /// </summary>
        public virtual string Summary { get; set; }

        /// <summary>
        /// Description.
        /// </summary>
        public virtual string[] Exceptions { get; set; } = new string[0];

        /// <summary>
        /// Status Code.
        /// </summary>
        public virtual int StatusCode { get; set; } = 500;

        /// <summary>
        /// Is Translated.
        /// </summary>
        public virtual bool IsTranslated { get; set; }

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

            var baseException = exception.GetBaseException();

            this.Summary = "Internal Server Error";
            this.StatusCode = (int)HttpStatusCode.InternalServerError;
            this.Exceptions = new[]
            {
                $"{baseException.GetType().Name} - {baseException.Message}"
            };

            switch (exception)
            {
                case AggregateException aggregateException:
                {
                    if (aggregateException.InnerException is TranslationException)
                    {
                        this.Exceptions = aggregateException.InnerExceptions
                            .Where(x => x is TranslationException)
                            .Select(x => x.Message)
                            .ToArray();

                        this.IsTranslated = true;
                    }
                    break;
                }
                case TranslationException _:
                    this.Exceptions = new[]
                    {
                        baseException.Message
                    };
                    this.IsTranslated = true;
                    break;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var exceptionsString = this.Exceptions
                .Aggregate($"Messages:{Environment.NewLine}", (current, exception) => current + exception + Environment.NewLine);

            return $"{this.StatusCode} {this.Summary}{Environment.NewLine}Messages:{Environment.NewLine}{exceptionsString}";
        }
    }
}