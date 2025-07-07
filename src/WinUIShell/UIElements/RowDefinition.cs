using WinUIShell.Common;

namespace WinUIShell;

public class RowDefinition : WinUIShellObject
{
    public double ActualHeight
    {
        get => PropertyAccessor.Get<double>(Id, nameof(ActualHeight))!;
    }

    public GridLength Height
    {
        get => PropertyAccessor.Get<GridLength>(Id, nameof(Height))!;
        set => PropertyAccessor.Set(Id, nameof(Height), value?.Id);
    }

    public double MaxHeight
    {
        get => PropertyAccessor.Get<double>(Id, nameof(MaxHeight))!;
        set => PropertyAccessor.Set(Id, nameof(MaxHeight), value);
    }

    public double MinHeight
    {
        get => PropertyAccessor.Get<double>(Id, nameof(MinHeight))!;
        set => PropertyAccessor.Set(Id, nameof(MinHeight), value);
    }

    public RowDefinition()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<RowDefinition>(),
            this);
    }
}
