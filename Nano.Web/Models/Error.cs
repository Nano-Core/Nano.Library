using System;
using System.Linq;
using System.Net;
using Nano.Models.Exceptions;

namespace Nano.Web.Models
{
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
        /// Status Code.
        /// </summary>
        public int StatusCode { get; set; } = 500;

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
                case TranslationException:
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