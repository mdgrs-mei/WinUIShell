using System.Management.Automation;
using WinUIShell.Common;

namespace WinUIShell;

public class UIElement : WinUIShellObject
{
    private readonly EventCallbackList _callbacks = new();

    public bool AllowDrop
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(AllowDrop))!;
        set => PropertyAccessor.Set(Id, nameof(AllowDrop), value);
    }

    public bool CanDrag
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(CanDrag))!;
        set => PropertyAccessor.Set(Id, nameof(CanDrag), value);
    }

    public double Opacity
    {
        get => PropertyAccessor.Get<double>(Id, nameof(Opacity))!;
        set => PropertyAccessor.Set(Id, nameof(Opacity), value);
    }

    public double RasterizationScale
    {
        get => PropertyAccessor.Get<double>(Id, nameof(RasterizationScale))!;
        set => PropertyAccessor.Set(Id, nameof(RasterizationScale), value);
    }

    public Microsoft.UI.Xaml.Visibility Visibility
    {
        get => PropertyAccessor.Get<Microsoft.UI.Xaml.Visibility>(Id, nameof(Visibility))!;
        set => PropertyAccessor.Set(Id, nameof(Visibility), value);
    }

    internal UIElement()
    {
    }

    internal UIElement(ObjectId id)
        : base(id)
    {
    }

    public bool Focus(FocusState value)
    {
        return CommandClient.Get().InvokeMethodAndGetResult<bool>(Id, nameof(Focus), value);
    }

    public void AddKeyDown(ScriptBlock scriptBlock, object? argumentList = null)
    {
        AddKeyDown(new EventCallback
        {
            ScriptBlock = scriptBlock,
            ArgumentList = argumentList
        });
    }
    public void AddKeyDown(EventCallback eventCallback)
    {
        _callbacks.Add(
            Id,
            "KeyDown",
            ObjectTypeMapping.Get().GetTargetTypeName<KeyRoutedEventArgs>(),
            eventCallback);
    }
}
