using WinUIShell.Common;

namespace WinUIShell;

public class RowDefinitionCollection : WinUIShellObjectList<RowDefinition>
{
    internal RowDefinitionCollection(ObjectId id)
        : base(id)
    {
    }
}
