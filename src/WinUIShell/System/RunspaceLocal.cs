using System.Collections.Concurrent;
using System.Management.Automation.Runspaces;

namespace WinUIShell;

internal sealed class RunspaceLocal<T>
{
    private readonly ConcurrentDictionary<int, T> _values = [];
    private readonly Func<T> _valueFactory;

    public T Value
    {
        get => _values.GetOrAdd(Runspace.DefaultRunspace.Id, (runspace) => _valueFactory());
        set => _values.AddOrUpdate(Runspace.DefaultRunspace.Id, value, (runspace, oldValue) => value);
    }

    public RunspaceLocal(Func<T> valueFactory)
    {
        _valueFactory = valueFactory;
    }
}
