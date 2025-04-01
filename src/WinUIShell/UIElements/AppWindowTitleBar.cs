using WinUIShell.Common;

namespace WinUIShell;

public class AppWindowTitleBar : WinUIShellObject
{
    public TitleBarTheme PreferredTheme
    {
        get => PropertyAccessor.Get<TitleBarTheme>(Id, nameof(PreferredTheme));
        set => PropertyAccessor.Set(Id, nameof(PreferredTheme), value);
    }

    internal AppWindowTitleBar(ObjectId id)
        : base(id)
    {
    }
}
