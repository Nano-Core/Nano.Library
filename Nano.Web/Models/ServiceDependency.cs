using Microsoft.Extensions.DependencyInjection;

namespace Nano.Web.Models
{
    /// <summary>
    /// Service Dependency.
    /// </summary>
    public class ServiceDependency
    {
        /// <summary>
        /// Service.
        /// </summary>
        public virtual string Service { get; set; }

        /// <summary>
        /// Implementation.
        /// </summary>
        public virtual string Implementation { get; set; }

        /// <summary>
        /// Life Time.
        /// </summary>
        public virtual ServiceLifetime LifeTime { get; set; }
    }
}