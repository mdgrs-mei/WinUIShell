using System.Management.Automation;
using WinUIShell.Common;

namespace WinUIShell;

public class TextBox : Control
{
    private readonly EventCallbackList _beforeTextChangingCallbacks = new();
    private readonly EventCallbackList _textChangedCallbacks = new();

    public bool AcceptsReturn
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(AcceptsReturn))!;
        set => PropertyAccessor.Set(Id, nameof(AcceptsReturn), value);
    }

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

    public bool IsReadOnly
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsReadOnly))!;
        set => PropertyAccessor.Set(Id, nameof(IsReadOnly), value);
    }

    public int MaxLength
    {
        get => PropertyAccessor.Get<int>(Id, nameof(MaxLength))!;
        set => PropertyAccessor.Set(Id, nameof(MaxLength), value);
    }

    public string PlaceholderText
    {
        get => PropertyAccessor.Get<string>(Id, nameof(PlaceholderText))!;
        set => PropertyAccessor.Set(Id, nameof(PlaceholderText), value);
    }

    public string SelectedText
    {
        get => PropertyAccessor.Get<string>(Id, nameof(SelectedText))!;
        set => PropertyAccessor.Set(Id, nameof(SelectedText), value);
    }

    public int SelectionLength
    {
        get => PropertyAccessor.Get<int>(Id, nameof(SelectionLength))!;
        set => PropertyAccessor.Set(Id, nameof(SelectionLength), value);
    }

    public int SelectionStart
    {
        get => PropertyAccessor.Get<int>(Id, nameof(SelectionStart))!;
        set => PropertyAccessor.Set(Id, nameof(SelectionStart), value);
    }

    public string Text
    {
        get => PropertyAccessor.Get<string>(Id, nameof(Text))!;
        set => PropertyAccessor.Set(Id, nameof(Text), value);
    }

    public TextAlignment TextAlignment
    {
        get => PropertyAccessor.Get<TextAlignment>(Id, nameof(TextAlignment))!;
        set => PropertyAccessor.Set(Id, nameof(TextAlignment), value);
    }

    public TextBox()
    {
        Id = CommandClient.Get().CreateObject(
            "Microsoft.UI.Xaml.Controls.TextBox, Microsoft.WinUI",
            this);
    }


    public void AddBeforeTextChanging(ScriptBlock scriptBlock, object? argumentList = null)
    {
        AddBeforeTextChanging(new EventCallback
        {
            ScriptBlock = scriptBlock,
            ArgumentList = argumentList
        });
    }
    public void AddBeforeTextChanging(EventCallback eventCallback)
    {
        _beforeTextChangingCallbacks.Add(
            "WinUIShell.Server.TextBoxAccessor, WinUIShell.Server",
            nameof(AddBeforeTextChanging),
            Id,
            eventCallback);
    }

    public void AddTextChanged(ScriptBlock scriptBlock, object? argumentList = null)
    {
        AddTextChanged(new EventCallback
        {
            ScriptBlock = scriptBlock,
            ArgumentList = argumentList
        });
    }
    public void AddTextChanged(EventCallback eventCallback)
    {
        _textChangedCallbacks.Add(
            "WinUIShell.Server.TextBoxAccessor, WinUIShell.Server",
            nameof(AddTextChanged),
            Id,
            eventCallback);
    }
}
