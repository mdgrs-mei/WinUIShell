using WinUIShell.Common;

namespace WinUIShell;

public class ResourceDictionary : WinUIShellObject
{
    public Resource? this[object key]
    {
        get => PropertyAccessor.GetIndexer<Resource>(Id, key);
    }

    internal ResourceDictionary(ObjectId id)
        : base(id)
    {
    }
}
