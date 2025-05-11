using WinUIShell.Common;

namespace WinUIShell;

public class UserControl : Control
{
    public UIElement? Content
    {
        get => PropertyAccessor.Get<UIElement>(Id, nameof(Content));
        set => PropertyAccessor.Set(Id, nameof(Content), value?.Id);
    }

    internal UserControl()
    {
    }

    internal UserControl(ObjectId id)
    : base(id)
    {
    }
}
