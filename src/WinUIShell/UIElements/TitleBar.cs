using WinUIShell.Common;

namespace WinUIShell;

public class TitleBar : Control
{
    public UIElement? Content
    {
        get => PropertyAccessor.Get<UIElement>(Id, nameof(Content));
        set => PropertyAccessor.Set(Id, nameof(Content), value?.Id);
    }

    public IconSource? IconSource
    {
        get => PropertyAccessor.Get<IconSource>(Id, nameof(IconSource));
        set => PropertyAccessor.Set(Id, nameof(IconSource), value?.Id);
    }

    public bool IsBackButtonVisible
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsBackButtonVisible))!;
        set => PropertyAccessor.Set(Id, nameof(IsBackButtonVisible), value);
    }

    public bool IsPaneToggleButtonVisible
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsPaneToggleButtonVisible))!;
        set => PropertyAccessor.Set(Id, nameof(IsPaneToggleButtonVisible), value);
    }

    public UIElement? LeftHeader
    {
        get => PropertyAccessor.Get<UIElement>(Id, nameof(LeftHeader));
        set => PropertyAccessor.Set(Id, nameof(LeftHeader), value?.Id);
    }

    public UIElement? RightHeader
    {
        get => PropertyAccessor.Get<UIElement>(Id, nameof(RightHeader));
        set => PropertyAccessor.Set(Id, nameof(RightHeader), value?.Id);
    }

    public string Subtitle
    {
        get => PropertyAccessor.Get<string>(Id, nameof(Subtitle))!;
        set => PropertyAccessor.Set(Id, nameof(Subtitle), value);
    }

    public string Title
    {
        get => PropertyAccessor.Get<string>(Id, nameof(Title))!;
        set => PropertyAccessor.Set(Id, nameof(Title), value);
    }

    public TitleBar()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<TitleBar>(),
            this);
    }

    internal TitleBar(ObjectId id)
        : base(id)
    {
    }
}
