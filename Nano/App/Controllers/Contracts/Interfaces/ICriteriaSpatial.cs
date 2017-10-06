using NetTopologySuite.Geometries;

namespace Nano.App.Controllers.Contracts.Interfaces
{
    /// <summary>
    /// Criteria interface.
    /// </summary>
    public interface ICriteriaSpatial : ICriteria
    {
        /// <summary>
        /// Geometry.
        /// </summary>
        Geometry Geometry { get; set; }
    }
}