using WinUIShell.Common;

namespace WinUIShell;

public class ScrollView : Control
{
    //public Visibility ComputedHorizontalScrollBarVisibility
    //public ScrollingScrollMode ComputedHorizontalScrollMode
    //public Visibility ComputedVerticalScrollBarVisibility
    //public ScrollingScrollMode ComputedVerticalScrollMode

    public UIElement? Content
    {
        get => PropertyAccessor.Get<UIElement>(Id, nameof(Content));
        set => PropertyAccessor.Set(Id, nameof(Content), value?.Id);
    }

    public ScrollingContentOrientation ContentOrientation
    {
        get => PropertyAccessor.Get<ScrollingContentOrientation>(Id, nameof(ContentOrientation))!;
        set => PropertyAccessor.Set(Id, nameof(ContentOrientation), value);
    }

    //public UIElement CurrentAnchor
    //public CompositionPropertySet ExpressionAnimationSources
    //public double ExtentHeight
    //public double ExtentWidth
    //public double HorizontalAnchorRatio
    //public double HorizontalOffset

    public ScrollingScrollBarVisibility HorizontalScrollBarVisibility
    {
        get => PropertyAccessor.Get<ScrollingScrollBarVisibility>(Id, nameof(HorizontalScrollBarVisibility))!;
        set => PropertyAccessor.Set(Id, nameof(HorizontalScrollBarVisibility), value);
    }

    public ScrollingChainMode HorizontalScrollChainMode
    {
        get => PropertyAccessor.Get<ScrollingChainMode>(Id, nameof(HorizontalScrollChainMode))!;
        set => PropertyAccessor.Set(Id, nameof(HorizontalScrollChainMode), value);
    }

    public ScrollingScrollMode HorizontalScrollMode
    {
        get => PropertyAccessor.Get<ScrollingScrollMode>(Id, nameof(HorizontalScrollMode))!;
        set => PropertyAccessor.Set(Id, nameof(HorizontalScrollMode), value);
    }

    public ScrollingRailMode HorizontalScrollRailMode
    {
        get => PropertyAccessor.Get<ScrollingRailMode>(Id, nameof(HorizontalScrollRailMode))!;
        set => PropertyAccessor.Set(Id, nameof(HorizontalScrollRailMode), value);
    }

    //public ScrollingInputKinds IgnoredInputKinds
    //public double MaxZoomFactor
    //public double MinZoomFactor
    //public ScrollPresenter ScrollPresenter
    //public double ScrollableHeight
    //public double ScrollableWidth
    //public ScrollingInteractionState State
    //public double VerticalAnchorRatio
    //public double VerticalOffset

    public ScrollingScrollBarVisibility VerticalScrollBarVisibility
    {
        get => PropertyAccessor.Get<ScrollingScrollBarVisibility>(Id, nameof(VerticalScrollBarVisibility))!;
        set => PropertyAccessor.Set(Id, nameof(VerticalScrollBarVisibility), value);
    }

    public ScrollingChainMode VerticalScrollChainMode
    {
        get => PropertyAccessor.Get<ScrollingChainMode>(Id, nameof(VerticalScrollChainMode))!;
        set => PropertyAccessor.Set(Id, nameof(VerticalScrollChainMode), value);
    }

    public ScrollingScrollMode VerticalScrollMode
    {
        get => PropertyAccessor.Get<ScrollingScrollMode>(Id, nameof(VerticalScrollMode))!;
        set => PropertyAccessor.Set(Id, nameof(VerticalScrollMode), value);
    }

    public ScrollingRailMode VerticalScrollRailMode
    {
        get => PropertyAccessor.Get<ScrollingRailMode>(Id, nameof(VerticalScrollRailMode))!;
        set => PropertyAccessor.Set(Id, nameof(VerticalScrollRailMode), value);
    }

    //public double ViewportHeight
    //public double ViewportWidth
    //public ScrollingChainMode ZoomChainMode
    //public float ZoomFactor
    //public ScrollingZoomMode ZoomMode

    public ScrollView()
    {
        Id = CommandClient.Get().CreateObject(
            "Microsoft.UI.Xaml.Controls.ScrollView, Microsoft.WinUI",
            this);
    }
}
