using System;

namespace Nano.Eventing.Attributes
{
    /// <summary>
    /// Publish Attribute.
    /// Types with this annotation, defines that an event will be published for the entity when it changes. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class PublishAttribute : Attribute
    {

    }
}