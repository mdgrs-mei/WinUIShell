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

    //public ScrollingContentOrientation ContentOrientation

    //public UIElement CurrentAnchor
    //public CompositionPropertySet ExpressionAnimationSources
    //public double ExtentHeight
    //public double ExtentWidth
    //public double HorizontalAnchorRatio
    //public double HorizontalOffset

    //public ScrollingScrollBarVisibility HorizontalScrollBarVisibility

    //public ScrollingChainMode HorizontalScrollChainMode
    //public ScrollingScrollMode HorizontalScrollMode
    //public ScrollingRailMode HorizontalScrollRailMode

    //public ScrollingInputKinds IgnoredInputKinds
    //public double MaxZoomFactor
    //public double MinZoomFactor
    //public ScrollPresenter ScrollPresenter
    //public double ScrollableHeight
    //public double ScrollableWidth
    //public ScrollingInteractionState State
    //public double VerticalAnchorRatio
    //public double VerticalOffset

    //public ScrollingScrollBarVisibility VerticalScrollBarVisibility
    //public ScrollingChainMode VerticalScrollChainMode
    //public ScrollingScrollMode VerticalScrollMode
    //public ScrollingRailMode VerticalScrollRailMode

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
