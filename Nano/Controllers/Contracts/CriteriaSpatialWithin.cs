namespace Nano.Controllers.Contracts
{
    /// <inheritdoc />
    public class CriteriaSpatialWithin : CriteriaSpatial
    {
        /// <summary>
        /// Radius.
        /// </summary>
        public virtual int Radius { get; set; } = 10000;
    }
}