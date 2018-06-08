using System;

namespace Nano.Eventing.Attributes
{
    // TODO: Publish / Subscribe Attributes, If you include a models package, where models has eventing attrbutes, those models would also get publish / subscribe event handlers configured. It's not good... Can we check if it's the core application or NuGet at runtime

    /// <summary>
    /// Subscribe Attribute.
    /// Types with this annotation, subscribes to events of the declaring type. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class SubscribeAttribute : Attribute
    {

    }
}