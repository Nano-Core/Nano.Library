namespace Nano.Data.Eventing.Models;

internal sealed class EntityEventingModel
{
    internal Paths Paths { get; } = new();

    internal Navigations Navigations { get; set; } = new();

    internal PropertyAccessors Accessors { get; } = new();

    internal ReversePublishPlans ReversePlans { get; set; } = new();
}