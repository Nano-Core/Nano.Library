namespace Nano.Data.Eventing.Models;

internal sealed class EntityEventingModel
{
    internal Navigations Navigations { get; set; } = new();

    internal PropertyAccessors Accessors { get; set; } = new();

    internal ReversePublishPlans ReversePlans { get; set; } = new();
}