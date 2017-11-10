using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Nano.Controllers.Criterias.Entities
{
    /// <summary>
    /// Pagination.
    /// </summary>
    public class Pagination
    {
        /// <summary>
        /// Number.
        /// </summary>
        public virtual int Number { get; set; } = 1;

        /// <summary>
        /// Count (Take).
        /// </summary>
        public virtual int Count { get; set; } = 25;

        /// <summary>
        /// Skip (Skip).
        /// </summary>
        internal virtual int Skip => (this.Number - 1) * this.Count;

        /// <summary>
        /// Parses a <see cref="HttpRequest"/> to a <see cref="Pagination"/> instance.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest"/>.</param>
        /// <returns>The <see cref="Pagination"/>.</returns>
        public static Pagination Parse(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var count = Pagination.GetCount(request);
            var number = Pagination.GetNumber(request);

            return new Pagination
            {
                Count = count,
                Number = number
            };
        }

        private static int GetCount(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var success = int.TryParse(request.Query["count"].FirstOrDefault(), out var count);
            if (!success)
                count = 25;

            return count;
        }
        private static int GetNumber(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var success = int.TryParse(request.Query["number"].FirstOrDefault(), out var number);
            if (!success)
                number = 1;

            return number;
        }
    }
}