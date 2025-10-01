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

    public Microsoft.UI.Xaml.Controls.ScrollingContentOrientation ContentOrientation
    {
        get => PropertyAccessor.Get<Microsoft.UI.Xaml.Controls.ScrollingContentOrientation>(Id, nameof(ContentOrientation))!;
        set => PropertyAccessor.Set(Id, nameof(ContentOrientation), value);
    }

    //public UIElement CurrentAnchor
    //public CompositionPropertySet ExpressionAnimationSources
    //public double ExtentHeight
    //public double ExtentWidth
    //public double HorizontalAnchorRatio
    //public double HorizontalOffset

    public Microsoft.UI.Xaml.Controls.ScrollingScrollBarVisibility HorizontalScrollBarVisibility
    {
        get => PropertyAccessor.Get<Microsoft.UI.Xaml.Controls.ScrollingScrollBarVisibility>(Id, nameof(HorizontalScrollBarVisibility))!;
        set => PropertyAccessor.Set(Id, nameof(HorizontalScrollBarVisibility), value);
    }

    public Microsoft.UI.Xaml.Controls.ScrollingChainMode HorizontalScrollChainMode
    {
        get => PropertyAccessor.Get<Microsoft.UI.Xaml.Controls.ScrollingChainMode>(Id, nameof(HorizontalScrollChainMode))!;
        set => PropertyAccessor.Set(Id, nameof(HorizontalScrollChainMode), value);
    }

    public Microsoft.UI.Xaml.Controls.ScrollingScrollMode HorizontalScrollMode
    {
        get => PropertyAccessor.Get<Microsoft.UI.Xaml.Controls.ScrollingScrollMode>(Id, nameof(HorizontalScrollMode))!;
        set => PropertyAccessor.Set(Id, nameof(HorizontalScrollMode), value);
    }

    public Microsoft.UI.Xaml.Controls.ScrollingRailMode HorizontalScrollRailMode
    {
        get => PropertyAccessor.Get<Microsoft.UI.Xaml.Controls.ScrollingRailMode>(Id, nameof(HorizontalScrollRailMode))!;
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

    public Microsoft.UI.Xaml.Controls.ScrollingScrollBarVisibility VerticalScrollBarVisibility
    {
        get => PropertyAccessor.Get<Microsoft.UI.Xaml.Controls.ScrollingScrollBarVisibility>(Id, nameof(VerticalScrollBarVisibility))!;
        set => PropertyAccessor.Set(Id, nameof(VerticalScrollBarVisibility), value);
    }

    public Microsoft.UI.Xaml.Controls.ScrollingChainMode VerticalScrollChainMode
    {
        get => PropertyAccessor.Get<Microsoft.UI.Xaml.Controls.ScrollingChainMode>(Id, nameof(VerticalScrollChainMode))!;
        set => PropertyAccessor.Set(Id, nameof(VerticalScrollChainMode), value);
    }

    public Microsoft.UI.Xaml.Controls.ScrollingScrollMode VerticalScrollMode
    {
        get => PropertyAccessor.Get<Microsoft.UI.Xaml.Controls.ScrollingScrollMode>(Id, nameof(VerticalScrollMode))!;
        set => PropertyAccessor.Set(Id, nameof(VerticalScrollMode), value);
    }

    public Microsoft.UI.Xaml.Controls.ScrollingRailMode VerticalScrollRailMode
    {
        get => PropertyAccessor.Get<Microsoft.UI.Xaml.Controls.ScrollingRailMode>(Id, nameof(VerticalScrollRailMode))!;
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
            ObjectTypeMapping.Get().GetTargetTypeName<ScrollView>(),
            this);
    }

    internal ScrollView(ObjectId id)
        : base(id)
    {
    }
}
