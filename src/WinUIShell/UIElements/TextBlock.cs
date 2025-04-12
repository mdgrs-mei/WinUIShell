﻿using WinUIShell.Common;

namespace WinUIShell;

public class TextBlock : FrameworkElement
{
    public double BaselineOffset
    {
        get => PropertyAccessor.Get<double>(Id, nameof(BaselineOffset))!;
    }

    public int CharacterSpacing
    {
        get => PropertyAccessor.Get<int>(Id, nameof(CharacterSpacing))!;
        set => PropertyAccessor.Set(Id, nameof(CharacterSpacing), value);
    }

    //public TextPointer ContentEnd
    //public TextPointer ContentStart
    //public FontFamily FontFamily

    public double FontSize
    {
        get => PropertyAccessor.Get<double>(Id, nameof(FontSize))!;
        set => PropertyAccessor.Set(Id, nameof(FontSize), value);
    }

    //public FontStretch FontStretch
    //public FontStyle FontStyle
    //public FontWeight FontWeight

    public Brush Foreground
    {
        get => PropertyAccessor.Get<Brush>(Id, nameof(Foreground))!;
        set => PropertyAccessor.Set(Id, nameof(Foreground), value?.Id);
    }

    public TextAlignment HorizontalTextAlignment
    {
        get => PropertyAccessor.Get<TextAlignment>(Id, nameof(HorizontalTextAlignment))!;
        set => PropertyAccessor.Set(Id, nameof(HorizontalTextAlignment), value);
    }

    //public InlineCollection Inlines

    public bool IsColorFontEnabled
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsColorFontEnabled))!;
        set => PropertyAccessor.Set(Id, nameof(IsColorFontEnabled), value);
    }

    public bool IsTextScaleFactorEnabled
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsTextScaleFactorEnabled))!;
        set => PropertyAccessor.Set(Id, nameof(IsTextScaleFactorEnabled), value);
    }

    public bool IsTextSelectionEnabled
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsTextSelectionEnabled))!;
        set => PropertyAccessor.Set(Id, nameof(IsTextSelectionEnabled), value);
    }

    public bool IsTextTrimmed
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsTextTrimmed))!;
    }

    public double LineHeight
    {
        get => PropertyAccessor.Get<double>(Id, nameof(LineHeight))!;
        set => PropertyAccessor.Set(Id, nameof(LineHeight), value);
    }

    //public LineStackingStrategy LineStackingStrategy

    public int MaxLines
    {
        get => PropertyAccessor.Get<int>(Id, nameof(MaxLines))!;
        set => PropertyAccessor.Set(Id, nameof(MaxLines), value);
    }

    //public OpticalMarginAlignment OpticalMarginAlignment

    public Thickness Padding
    {
        get => PropertyAccessor.Get<Thickness>(Id, nameof(Padding))!;
        set => PropertyAccessor.Set(Id, nameof(Padding), value?.Id);
    }

    public string SelectedText
    {
        get => PropertyAccessor.Get<string>(Id, nameof(SelectedText))!;
    }

    //public TextPointer SelectionEnd
    //public FlyoutBase SelectionFlyout

    public SolidColorBrush SelectionHighlightColor
    {
        get => PropertyAccessor.Get<SolidColorBrush>(Id, nameof(SelectionHighlightColor))!;
        set => PropertyAccessor.Set(Id, nameof(SelectionHighlightColor), value?.Id);
    }

    //public TextPointer SelectionStart

    public string Text
    {
        get => PropertyAccessor.Get<string>(Id, nameof(Text))!;
        set => PropertyAccessor.Set(Id, nameof(Text), value);
    }

    //Deprecated
    //public TextAlignment TextAlignment

    //public TextDecorations TextDecorations
    //public IList<TextHighlighter> TextHighlighters
    //public TextLineBounds TextLineBounds
    //public TextReadingOrder TextReadingOrder
    //public TextTrimming TextTrimming
    //public TextWrapping TextWrapping

    public TextBlock()
    {
        Id = CommandClient.Get().CreateObject(
            "Microsoft.UI.Xaml.Controls.TextBlock, Microsoft.WinUI",
            this);
    }
}
