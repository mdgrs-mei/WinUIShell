using WinUIShell.Common;
namespace WinUIShell;

public class FrameworkElement : UIElement
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

    public string Name
    {
        get => PropertyAccessor.Get<string>(Id, nameof(Name))!;
        set => PropertyAccessor.Set(Id, nameof(Name), value);
    }

    public Microsoft.UI.Xaml.HorizontalAlignment HorizontalAlignment
    {
        get => PropertyAccessor.Get<Microsoft.UI.Xaml.HorizontalAlignment>(Id, nameof(HorizontalAlignment))!;
        set => PropertyAccessor.Set(Id, nameof(HorizontalAlignment), value);
    }

    public Microsoft.UI.Xaml.ElementTheme RequestedTheme
    {
        get => PropertyAccessor.Get<Microsoft.UI.Xaml.ElementTheme>(Id, nameof(RequestedTheme))!;
        set => PropertyAccessor.Set(Id, nameof(RequestedTheme), value);
    }

    public Style? Style
    {
        get => PropertyAccessor.Get<Style>(Id, nameof(Style));
        set => PropertyAccessor.Set(Id, nameof(Style), value?.Id);
    }

    public object? Tag
    {
        get => PropertyAccessor.Get<object>(Id, nameof(Tag));
        set
        {
            if (value is WinUIShellObject v)
            {
                PropertyAccessor.Set(Id, nameof(Tag), v.Id);
            }
            else
            {
                PropertyAccessor.Set(Id, nameof(Tag), value);
            }
        }
    }

    public Microsoft.UI.Xaml.VerticalAlignment VerticalAlignment
    {
        get => PropertyAccessor.Get<Microsoft.UI.Xaml.VerticalAlignment>(Id, nameof(VerticalAlignment))!;
        set => PropertyAccessor.Set(Id, nameof(VerticalAlignment), value);
    }

    public double Width
    {
        get => PropertyAccessor.Get<double>(Id, nameof(Width))!;
        set => PropertyAccessor.Set(Id, nameof(Width), value);
    }

    internal FrameworkElement()
    {
    }

    internal FrameworkElement(ObjectId id)
        : base(id)
    {
    }

    public object? FindName(string name)
    {
        return CommandClient.Get().InvokeMethodAndGetResult<object?>(Id, nameof(FindName), name);
    }
}
