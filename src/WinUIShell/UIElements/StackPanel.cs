using WinUIShell.Common;

namespace WinUIShell;

public class StackPanel : Panel
{
    public BackgroundSizing BackgroundSizing
    {
        get => PropertyAccessor.Get<BackgroundSizing>(Id, nameof(BackgroundSizing))!;
        set => PropertyAccessor.Set(Id, nameof(BackgroundSizing), value);
    }

    public Brush BorderBrush
    {
        get => PropertyAccessor.Get<Brush>(Id, nameof(BorderBrush))!;
        set => PropertyAccessor.Set(Id, nameof(BorderBrush), value?.Id);
    }

    public Thickness BorderThickness
    {
        get => PropertyAccessor.Get<Thickness>(Id, nameof(BorderThickness))!;
        set => PropertyAccessor.Set(Id, nameof(BorderThickness), value?.Id);
    }

    public CornerRadius CornerRadius
    {
        get => PropertyAccessor.Get<CornerRadius>(Id, nameof(CornerRadius))!;
        set => PropertyAccessor.Set(Id, nameof(CornerRadius), value?.Id);
    }

    public Orientation Orientation
    {
        get => PropertyAccessor.Get<Orientation>(Id, nameof(Orientation))!;
        set => PropertyAccessor.Set(Id, nameof(Orientation), value);
    }

    public Thickness Padding
    {
        get => PropertyAccessor.Get<Thickness>(Id, nameof(Padding))!;
        set => PropertyAccessor.Set(Id, nameof(Padding), value?.Id);
    }

    public double Spacing
    {
        get => PropertyAccessor.Get<double>(Id, nameof(Spacing))!;
        set => PropertyAccessor.Set(Id, nameof(Spacing), value);
    }

    public StackPanel()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<StackPanel>(),
            this);
    }

    internal StackPanel(ObjectId id)
        : base(id)
    {
    }
}
