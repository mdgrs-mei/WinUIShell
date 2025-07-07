using System.Management.Automation;
using WinUIShell.Common;

namespace WinUIShell;

public class TextBox : Control
{
    private const string _accessorClassName = "WinUIShell.Server.TextBoxAccessor, WinUIShell.Server";
    private readonly EventCallbackList _beforeTextChangingCallbacks = new(_accessorClassName);
    private readonly EventCallbackList _textChangedCallbacks = new(_accessorClassName);

    public bool AcceptsReturn
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(AcceptsReturn))!;
        set => PropertyAccessor.Set(Id, nameof(AcceptsReturn), value);
    }

    public bool CanPasteClipboardContent
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(CanPasteClipboardContent))!;
    }

    public bool CanRedo
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(CanRedo))!;
    }

    public bool CanUndo
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(CanUndo))!;
    }

    public CharacterCasing CharacterCasing
    {
        get => PropertyAccessor.Get<CharacterCasing>(Id, nameof(CharacterCasing))!;
        set => PropertyAccessor.Set(Id, nameof(CharacterCasing), value);
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

    public CandidateWindowAlignment DesiredCandidateWindowAlignment
    {
        get => PropertyAccessor.Get<CandidateWindowAlignment>(Id, nameof(DesiredCandidateWindowAlignment))!;
        set => PropertyAccessor.Set(Id, nameof(DesiredCandidateWindowAlignment), value);
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

    public TextAlignment HorizontalTextAlignment
    {
        get => PropertyAccessor.Get<TextAlignment>(Id, nameof(HorizontalTextAlignment))!;
        set => PropertyAccessor.Set(Id, nameof(HorizontalTextAlignment), value);
    }

    //public InputScope InputScope

    public bool IsColorFontEnabled
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsColorFontEnabled))!;
        set => PropertyAccessor.Set(Id, nameof(IsColorFontEnabled), value);
    }

    public bool IsReadOnly
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsReadOnly))!;
        set => PropertyAccessor.Set(Id, nameof(IsReadOnly), value);
    }

    public bool IsSpellCheckEnabled
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsSpellCheckEnabled))!;
        set => PropertyAccessor.Set(Id, nameof(IsSpellCheckEnabled), value);
    }

    public bool IsTextPredictionEnabled
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsTextPredictionEnabled))!;
        set => PropertyAccessor.Set(Id, nameof(IsTextPredictionEnabled), value);
    }

    public int MaxLength
    {
        get => PropertyAccessor.Get<int>(Id, nameof(MaxLength))!;
        set => PropertyAccessor.Set(Id, nameof(MaxLength), value);
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

    public bool PreventKeyboardDisplayOnProgrammaticFocus
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(PreventKeyboardDisplayOnProgrammaticFocus))!;
        set => PropertyAccessor.Set(Id, nameof(PreventKeyboardDisplayOnProgrammaticFocus), value);
    }

    //public FlyoutBase ProofingMenuFlyout

    public string SelectedText
    {
        get => PropertyAccessor.Get<string>(Id, nameof(SelectedText))!;
        set => PropertyAccessor.Set(Id, nameof(SelectedText), value);
    }

    //public FlyoutBase SelectionFlyout

    public SolidColorBrush SelectionHighlightColor
    {
        get => PropertyAccessor.Get<SolidColorBrush>(Id, nameof(SelectionHighlightColor))!;
        set => PropertyAccessor.Set(Id, nameof(SelectionHighlightColor), value?.Id);
    }

    public SolidColorBrush SelectionHighlightColorWhenNotFocused
    {
        get => PropertyAccessor.Get<SolidColorBrush>(Id, nameof(SelectionHighlightColorWhenNotFocused))!;
        set => PropertyAccessor.Set(Id, nameof(SelectionHighlightColorWhenNotFocused), value?.Id);
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

    public TextReadingOrder TextReadingOrder
    {
        get => PropertyAccessor.Get<TextReadingOrder>(Id, nameof(TextReadingOrder))!;
        set => PropertyAccessor.Set(Id, nameof(TextReadingOrder), value);
    }

    public TextWrapping TextWrapping
    {
        get => PropertyAccessor.Get<TextWrapping>(Id, nameof(TextWrapping))!;
        set => PropertyAccessor.Set(Id, nameof(TextWrapping), value);
    }

    public TextBox()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<TextBox>(),
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
            nameof(AddTextChanged),
            Id,
            eventCallback);
    }
}
