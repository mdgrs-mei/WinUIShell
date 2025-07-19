using WinUIShell.Common;

namespace WinUIShell;

public class ResourceDictionary : WinUIShellObject
{
    public object? this[object key]
    {
        get => PropertyAccessor.GetIndexer<object>(Id, key);
    }

    internal ResourceDictionary(ObjectId id)
        : base(id)
    {
    }
}
