using WinUIShell.Common;

namespace WinUIShell;

public class ColumnDefinitionCollection : WinUIShellObjectList<ColumnDefinition>
{
    internal ColumnDefinitionCollection(ObjectId id)
        : base(id)
    {
    }
}
