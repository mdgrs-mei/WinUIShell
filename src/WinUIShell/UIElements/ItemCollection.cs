using WinUIShell.Common;

namespace WinUIShell;

public class ItemCollection : WinUIShellObjectList<object>
{
    internal ItemCollection(ObjectId id)
        : base(id)
    {
    }
}
