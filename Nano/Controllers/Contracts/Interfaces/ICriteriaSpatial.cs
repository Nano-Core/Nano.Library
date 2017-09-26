using System.Collections.Generic;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using Location = Nano.Models.Types.Location;

namespace Nano.Controllers.Contracts.Interfaces
{
    /// <summary>
    /// Criteria Spatial interface.
    /// </summary>
    public interface ICriteriaSpatial : ICriteria
    {
        /// <summary>
        /// Geometry.
        /// Converts <see cref="Locations"/> into an array of <see cref="Point"/>'s, 
        /// and uses it to construct a valid <see cref="IGeometry"/> instance.
        /// </summary>
        IGeometry Geometry { get; }

        /// <summary>
        /// Locations.
        /// </summary>
        IList<Location> Locations { get; set; }
    }
}