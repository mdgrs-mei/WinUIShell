using WinUIShell.Common;

namespace WinUIShell;

public class ProgressRing : Control
{
    public bool IsActive
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsActive))!;
        set => PropertyAccessor.Set(Id, nameof(IsActive), value);
    }

    public bool IsIndeterminate
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsIndeterminate))!;
        set => PropertyAccessor.Set(Id, nameof(IsIndeterminate), value);
    }

    public double Maximum
    {
        get => PropertyAccessor.Get<double>(Id, nameof(Maximum))!;
        set => PropertyAccessor.Set(Id, nameof(Maximum), value);
    }

    public double Minimum
    {
        get => PropertyAccessor.Get<double>(Id, nameof(Minimum))!;
        set => PropertyAccessor.Set(Id, nameof(Minimum), value);
    }

    public double Value
    {
        get => PropertyAccessor.Get<double>(Id, nameof(Value))!;
        set => PropertyAccessor.Set(Id, nameof(Value), value);
    }

    public ProgressRing()
    {
        Id = CommandClient.Get().CreateObject(
            "Microsoft.UI.Xaml.Controls.ProgressRing, Microsoft.WinUI",
            this);
    }
}
