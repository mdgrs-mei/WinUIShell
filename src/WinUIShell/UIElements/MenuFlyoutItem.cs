using System.Management.Automation;
using WinUIShell.Common;

namespace WinUIShell;

public class MenuFlyoutItem : MenuFlyoutItemBase
{
    private const string _accessorClassName = "WinUIShell.Server.MenuFlyoutItemAccessor, WinUIShell.Server";
    private readonly EventCallbackList _clickCallbacks = new(_accessorClassName);

    public IconElement? Icon
    {
        get => PropertyAccessor.Get<IconElement>(Id, nameof(Icon));
        set => PropertyAccessor.Set(Id, nameof(Icon), value?.Id);
    }

    //public string KeyboardAcceleratorTextOverride
    //public MenuFlyoutItemTemplateSettings TemplateSettings => IMenuFlyoutItemMethods.get_TemplateSettings(_objRef_global__Microsoft_UI_Xaml_Controls_IMenuFlyoutItem);

    public string Text
    {
        get => PropertyAccessor.Get<string>(Id, nameof(Text))!;
        set => PropertyAccessor.Set(Id, nameof(Text), value);
    }

    public MenuFlyoutItem()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<MenuFlyoutItem>(),
            this);
    }

    internal MenuFlyoutItem(ObjectId id)
        : base(id)
    {
    }

    public void AddClick(ScriptBlock scriptBlock, object? argumentList = null)
    {
        AddClick(new EventCallback
        {
            ScriptBlock = scriptBlock,
            ArgumentList = argumentList
        });
    }
    public void AddClick(EventCallback eventCallback)
    {
        _clickCallbacks.Add(
            nameof(AddClick),
            Id,
            eventCallback);
    }
}
