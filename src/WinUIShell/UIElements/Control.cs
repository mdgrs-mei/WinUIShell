using WinUIShell.Common;
namespace WinUIShell;

public class Control : FrameworkElement
{
    public Brush Background
    {
        get => PropertyAccessor.Get<Brush>(Id, nameof(Background))!;
        set => PropertyAccessor.Set(Id, nameof(Background), value?.Id);
    }

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

    public int CharacterSpacing
    {
        get => PropertyAccessor.Get<int>(Id, nameof(CharacterSpacing))!;
        set => PropertyAccessor.Set(Id, nameof(CharacterSpacing), value);
    }

    public CornerRadius CornerRadius
    {
        get => PropertyAccessor.Get<CornerRadius>(Id, nameof(CornerRadius))!;
        set => PropertyAccessor.Set(Id, nameof(CornerRadius), value?.Id);
    }

    public FontFamily FontFamily
    {
        get => PropertyAccessor.Get<FontFamily>(Id, nameof(FontFamily))!;
        set => PropertyAccessor.Set(Id, nameof(FontFamily), value?.Id);
    }

    public double FontSize
    {
        get => PropertyAccessor.Get<double>(Id, nameof(FontSize))!;
        set => PropertyAccessor.Set(Id, nameof(FontSize), value);
    }

    public FontStretch FontStretch
    {
        get => PropertyAccessor.Get<FontStretch>(Id, nameof(FontStretch))!;
        set => PropertyAccessor.Set(Id, nameof(FontStretch), value);
    }

    public FontStyle FontStyle
    {
        get => PropertyAccessor.Get<FontStyle>(Id, nameof(FontStyle))!;
        set => PropertyAccessor.Set(Id, nameof(FontStyle), value);
    }

    public FontWeight FontWeight
    {
        get => PropertyAccessor.Get<FontWeight>(Id, nameof(FontWeight))!;
        set => PropertyAccessor.Set(Id, nameof(FontWeight), value?.Id);
    }

    public Brush Foreground
    {
        get => PropertyAccessor.Get<Brush>(Id, nameof(Foreground))!;
        set => PropertyAccessor.Set(Id, nameof(Foreground), value?.Id);
    }

    public HorizontalAlignment HorizontalContentAlignment
    {
        get => PropertyAccessor.Get<HorizontalAlignment>(Id, nameof(HorizontalContentAlignment))!;
        set => PropertyAccessor.Set(Id, nameof(HorizontalContentAlignment), value);
    }

    public bool IsEnabled
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsEnabled))!;
        set => PropertyAccessor.Set(Id, nameof(IsEnabled), value);
    }

    public bool IsFocusEngaged
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsFocusEngaged))!;
        set => PropertyAccessor.Set(Id, nameof(IsFocusEngaged), value);
    }

    public bool IsFocusEngagementEnabled
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsFocusEngagementEnabled))!;
        set => PropertyAccessor.Set(Id, nameof(IsFocusEngagementEnabled), value);
    }

    public bool IsTextScaleFactorEnabled
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsTextScaleFactorEnabled))!;
        set => PropertyAccessor.Set(Id, nameof(IsTextScaleFactorEnabled), value);
    }

    public Thickness Padding
    {
        get => PropertyAccessor.Get<Thickness>(Id, nameof(Padding))!;
        set => PropertyAccessor.Set(Id, nameof(Padding), value?.Id);
    }

    public VerticalAlignment VerticalContentAlignment
    {
        get => PropertyAccessor.Get<VerticalAlignment>(Id, nameof(VerticalContentAlignment))!;
        set => PropertyAccessor.Set(Id, nameof(VerticalContentAlignment), value);
    }

    internal Control()
    {
    }

    internal Control(ObjectId id)
        : base(id)
    {
    }
}
