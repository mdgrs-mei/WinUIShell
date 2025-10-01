using WinUIShell.Common;

namespace WinUIShell;

public class AppWindowTitleBar : WinUIShellObject
{
    public Microsoft.UI.Windowing.TitleBarTheme PreferredTheme
    {
        get => PropertyAccessor.Get<Microsoft.UI.Windowing.TitleBarTheme>(Id, nameof(PreferredTheme));
        set => PropertyAccessor.Set(Id, nameof(PreferredTheme), value);
    }

    internal AppWindowTitleBar(ObjectId id)
        : base(id)
    {
    }
}
