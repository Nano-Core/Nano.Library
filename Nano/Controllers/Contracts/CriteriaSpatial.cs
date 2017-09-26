using System.Collections.Generic;
using System.Linq;
using GeoAPI.Geometries;
using Nano.Controllers.Contracts.Interfaces;
using NetTopologySuite.Geometries;
using Location = Nano.Models.Types.Location;

namespace Nano.Controllers.Contracts
{
    /// <inheritdoc cref="ICriteriaSpatial"/>
    public class CriteriaSpatial : Criteria, ICriteriaSpatial
    {
        /// <summary>
        /// Geometry Factory.
        /// </summary>
        protected internal GeometryFactory Factory => new GeometryFactory(new PrecisionModel(PrecisionModels.Floating), 4326);

        /// <inheritdoc />
        public virtual IGeometry Geometry
        {
            get
            {
                var points = this.Locations
                    .Select(x => new Point(x.Longitude, x.Latitude))
                    .ToArray();

                return points.Any()
                    ? this.Factory.BuildGeometry(points)
                    : null;
            }
        }

        /// <inheritdoc />
        public virtual IList<Location> Locations { get; set; } = new List<Location>();
    }
}