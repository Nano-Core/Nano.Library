using System;

namespace Nano.Eventing.Attributes
{
    // TODO: Subscribe Attribute will register event handlers for included models (with Subscribe annotation). How to avoid?

    /// <summary>
    /// Subscribe Attribute.
    /// Types with this annotation, subscribes to events of the declaring type. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class SubscribeAttribute : Attribute
    {

    }
}