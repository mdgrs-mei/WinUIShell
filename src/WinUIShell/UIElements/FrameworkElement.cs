﻿namespace WinUIShell;

public abstract class FrameworkElement : UIElement
{
    public double Height
    {
        get => PropertyAccessor.Get<double>(Id, nameof(Height))!;
        set => PropertyAccessor.Set(Id, nameof(Height), value);
    }

    public Thickness Margin
    {
        get => PropertyAccessor.Get<Thickness>(Id, nameof(Margin))!;
        set => PropertyAccessor.Set(Id, nameof(Margin), value?.Id);
    }

    public HorizontalAlignment HorizontalAlignment
    {
        get => PropertyAccessor.Get<HorizontalAlignment>(Id, nameof(HorizontalAlignment))!;
        set => PropertyAccessor.Set(Id, nameof(HorizontalAlignment), value);
    }

    public Style? Style
    {
        get => PropertyAccessor.Get<Style>(Id, nameof(Style));
        set => PropertyAccessor.Set(Id, nameof(Style), value?.Id);
    }

    public VerticalAlignment VerticalAlignment
    {
        get => PropertyAccessor.Get<VerticalAlignment>(Id, nameof(VerticalAlignment))!;
        set => PropertyAccessor.Set(Id, nameof(VerticalAlignment), value);
    }

    public double Width
    {
        get => PropertyAccessor.Get<double>(Id, nameof(Width))!;
        set => PropertyAccessor.Set(Id, nameof(Width), value);
    }
}
