using WinUIShell.Common;

namespace WinUIShell;

public class ProgressBar : RangeBase
{
    public bool IsIndeterminate
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsIndeterminate))!;
        set => PropertyAccessor.Set(Id, nameof(IsIndeterminate), value);
    }

    public bool ShowError
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(ShowError))!;
        set => PropertyAccessor.Set(Id, nameof(ShowError), value);
    }

    public bool ShowPaused
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(ShowPaused))!;
        set => PropertyAccessor.Set(Id, nameof(ShowPaused), value);
    }

    public ProgressBar()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<ProgressBar>(),
            this);
    }
}
