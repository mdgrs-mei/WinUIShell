using WinUIShell.Common;

namespace WinUIShell;

public class UIElementCollection : WinUIShellObjectList<UIElement>
{
    internal UIElementCollection(ObjectId id)
        : base(id)
    {
    }

    public void Move(uint oldIndex, uint newIndex)
    {
        CommandClient.Get().InvokeMethod(Id, nameof(Move), oldIndex, newIndex);
    }
}
