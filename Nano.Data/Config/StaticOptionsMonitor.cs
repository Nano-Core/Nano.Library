using System;
using Microsoft.Extensions.Options;

namespace Nano.Data.Config;

internal sealed class StaticOptionsMonitor<T> : IOptionsMonitor<T>
{
    internal StaticOptionsMonitor(T value)
    {
        this.CurrentValue = value;
    }

    public T CurrentValue { get; }

    public T Get(string name) => CurrentValue;

    public IDisposable OnChange(Action<T, string> listener) => NullDisposable.Instance;

    private sealed class NullDisposable : IDisposable
    {
        public static readonly IDisposable Instance = new NullDisposable();

        public void Dispose()
        {
        }
    }
}