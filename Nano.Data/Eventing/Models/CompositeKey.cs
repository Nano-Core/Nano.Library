using System;
using System.Linq;

namespace Nano.Data.Eventing.Models;

internal sealed class CompositeKey : IEquatable<CompositeKey>
{
    private readonly int hashCode;

    public CompositeKey(object[] values)
    {
        this.Values = values;

        var hash = new HashCode();

        foreach (var value in this.Values)
        {
            hash
                .Add(value);
        }

        this.hashCode = hash
            .ToHashCode();
    }

    public object[] Values { get; }

    public bool Equals(CompositeKey? other)
    {
        if (other == null || other.Values.Length != this.Values.Length)
        {
            return false;
        }

        return !this.Values
            .Where((x, i) => !Equals(x, other.Values[i]))
            .Any();
    }

    public override bool Equals(object? obj) => this.Equals(obj as CompositeKey);

    public override int GetHashCode() => this.hashCode;
}