using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Nano.Controllers.Criterias.Enums;

namespace Nano.Controllers.Criterias.Entities
{
    /// <summary>
    /// Ordering.
    /// </summary>
    public class Ordering
    {
        /// <summary>
        /// By.
        /// </summary>
        public virtual string By { get; set; } = "Id";

        /// <summary>
        /// Direction.
        /// </summary>
        public virtual SortDirection Direction { get; set; } = SortDirection.Asc;

        /// <summary>
        /// Parses a <see cref="HttpRequest"/> to a <see cref="Ordering"/> instance.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest"/>.</param>
        /// <returns>The <see cref="Ordering"/>.</returns>
        public static Ordering Parse(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var by = Ordering.GetBy(request);
            var direction = Ordering.GetDirection(request);

            return new Ordering
            {
                By = by,
                Direction = direction
            };
        }

        private static string GetBy(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var orderBy = request.Query["by"].FirstOrDefault() ?? "Id";

            return orderBy;
        }
        private static SortDirection GetDirection(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var success = Enum.TryParse<SortDirection>(request.Query["direction"].FirstOrDefault(), true, out var direction);
            if (!success)
                direction = SortDirection.Asc;

            return direction;
        }
    }
}