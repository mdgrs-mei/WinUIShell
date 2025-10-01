using System.Management.Automation;
using WinUIShell.Common;

namespace WinUIShell;

public class ComboBox : Selector
{
    private readonly EventCallbackList _callbacks = new();

    public object? Description
    {
        get => PropertyAccessor.Get<object>(Id, nameof(Description));
        set
        {
            if (value is WinUIShellObject v)
            {
                PropertyAccessor.Set(Id, nameof(Description), v.Id);
            }
            else
            {
                PropertyAccessor.Set(Id, nameof(Description), value);
            }
        }
    }

    public object? Header
    {
        get => PropertyAccessor.Get<object>(Id, nameof(Header));
        set
        {
            if (value is WinUIShellObject v)
            {
                PropertyAccessor.Set(Id, nameof(Header), v.Id);
            }
            else
            {
                PropertyAccessor.Set(Id, nameof(Header), value);
            }
        }
    }

    //public DataTemplate HeaderTemplate


    public bool IsDropDownOpen
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsDropDownOpen))!;
        set => PropertyAccessor.Set(Id, nameof(IsDropDownOpen), value);
    }

    public bool IsEditable
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsEditable))!;
        set => PropertyAccessor.Set(Id, nameof(IsEditable), value);
    }

    public bool IsSelectionBoxHighlighted
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsSelectionBoxHighlighted))!;
    }

    public bool IsTextSearchEnabled
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsTextSearchEnabled))!;
        set => PropertyAccessor.Set(Id, nameof(IsTextSearchEnabled), value);
    }

    public Microsoft.UI.Xaml.Controls.LightDismissOverlayMode LightDismissOverlayMode
    {
        get => PropertyAccessor.Get<Microsoft.UI.Xaml.Controls.LightDismissOverlayMode>(Id, nameof(LightDismissOverlayMode))!;
        set => PropertyAccessor.Set(Id, nameof(LightDismissOverlayMode), value);
    }

    public double MaxDropDownHeight
    {
        get => PropertyAccessor.Get<double>(Id, nameof(MaxDropDownHeight))!;
        set => PropertyAccessor.Set(Id, nameof(MaxDropDownHeight), value);
    }

    public Brush PlaceholderForeground
    {
        get => PropertyAccessor.Get<Brush>(Id, nameof(PlaceholderForeground))!;
        set => PropertyAccessor.Set(Id, nameof(PlaceholderForeground), value?.Id);
    }

    public string PlaceholderText
    {
        get => PropertyAccessor.Get<string>(Id, nameof(PlaceholderText))!;
        set => PropertyAccessor.Set(Id, nameof(PlaceholderText), value);
    }

    public object? SelectionBoxItem
    {
        get => PropertyAccessor.Get<object>(Id, nameof(SelectionBoxItem));
    }

    //public DataTemplate SelectionBoxItemTemplate => IComboBoxMethods.get_SelectionBoxItemTemplate(_objRef_global__Microsoft_UI_Xaml_Controls_IComboBox);

    public Microsoft.UI.Xaml.Controls.ComboBoxSelectionChangedTrigger SelectionChangedTrigger
    {
        get => PropertyAccessor.Get<Microsoft.UI.Xaml.Controls.ComboBoxSelectionChangedTrigger>(Id, nameof(SelectionChangedTrigger))!;
        set => PropertyAccessor.Set(Id, nameof(SelectionChangedTrigger), value);
    }

    //public ComboBoxTemplateSettings TemplateSettings => IComboBoxMethods.get_TemplateSettings(_objRef_global__Microsoft_UI_Xaml_Controls_IComboBox);

    public string Text
    {
        get => PropertyAccessor.Get<string>(Id, nameof(Text))!;
        set => PropertyAccessor.Set(Id, nameof(Text), value);
    }

    public Style TextBoxStyle
    {
        get => PropertyAccessor.Get<Style>(Id, nameof(TextBoxStyle))!;
        set => PropertyAccessor.Set(Id, nameof(TextBoxStyle), value?.Id);
    }

    public ComboBox()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<ComboBox>(),
            this);
    }

    internal ComboBox(ObjectId id)
        : base(id)
    {
    }

    public void AddDropDownClosed(ScriptBlock scriptBlock, object? argumentList = null)
    {
        AddDropDownClosed(new EventCallback
        {
            ScriptBlock = scriptBlock,
            ArgumentList = argumentList
        });
    }
    public void AddDropDownClosed(EventCallback eventCallback)
    {
        _callbacks.Add(
            Id,
            "DropDownClosed",
            ObjectTypeMapping.Get().GetTargetTypeName<object>(),
            eventCallback);
    }

    public void AddDropDownOpened(ScriptBlock scriptBlock, object? argumentList = null)
    {
        AddDropDownOpened(new EventCallback
        {
            ScriptBlock = scriptBlock,
            ArgumentList = argumentList
        });
    }
    public void AddDropDownOpened(EventCallback eventCallback)
    {
        _callbacks.Add(
            Id,
            "DropDownOpened",
            ObjectTypeMapping.Get().GetTargetTypeName<object>(),
            eventCallback);
    }

    public void AddTextSubmitted(ScriptBlock scriptBlock, object? argumentList = null)
    {
        AddTextSubmitted(new EventCallback
        {
            ScriptBlock = scriptBlock,
            ArgumentList = argumentList
        });
    }
    public void AddTextSubmitted(EventCallback eventCallback)
    {
        _callbacks.Add(
            Id,
            "TextSubmitted",
            ObjectTypeMapping.Get().GetTargetTypeName<ComboBoxTextSubmittedEventArgs>(),
            eventCallback);
    }
}
