using System;
using System.Linq;

namespace Nano.Data.Eventing.Models;

internal sealed class CompositeKey : IEquatable<CompositeKey>
{
    private readonly int hashCode;
    private readonly object[] values;

    public CompositeKey(object[] values)
    {
        this.values = values;

        var hash = new HashCode();

        foreach (var value in this.values)
        {
            hash
                .Add(value);
        }

        this.hashCode = hash
            .ToHashCode();
    }

    public object[] Values => this.values;

    public bool Equals(CompositeKey? other)
    {
        if (other == null || other.values.Length != this.values.Length)
        {
            return false;
        }

        return !this.values
            .Where((x, i) => !Equals(x, other.values[i]))
            .Any();
    }

    public override bool Equals(object? obj) => this.Equals(obj as CompositeKey);

    public override int GetHashCode() => this.hashCode;
}