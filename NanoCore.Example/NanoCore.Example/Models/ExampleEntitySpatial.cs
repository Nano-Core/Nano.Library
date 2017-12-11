using System.ComponentModel.DataAnnotations;
using Nano.Models;

namespace NanoCore.Example.Models
{
    /// <summary>
    /// Example Entity Spatial.
    /// </summary>
    public class ExampleEntitySpatial : DefaultEntitySpatial
    {
        /// <summary>
        /// Property One.
        /// </summary>
        [Required]
        public virtual string PropertyOne { get; set; }
    }
}