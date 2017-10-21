using System;

namespace Nano.Eventing.Attributes
{
    /// <summary>
    /// Eventing Attribute.
    /// Types annotated with this <see cref="Attribute"/> will publish a message changes to the instance is saved. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class EventingAttribute : Attribute
    {

    }
}