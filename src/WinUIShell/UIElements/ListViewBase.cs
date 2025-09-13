using System.Management.Automation;
using WinUIShell.Common;

namespace WinUIShell;

public class ListViewBase : Selector
{
    private readonly EventCallbackList _callbacks = new();

    public bool CanDragItems
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(CanDragItems))!;
        set => PropertyAccessor.Set(Id, nameof(CanDragItems), value);
    }

    public bool CanReorderItems
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(CanReorderItems))!;
        set => PropertyAccessor.Set(Id, nameof(CanReorderItems), value);
    }

    public double DataFetchSize
    {
        get => PropertyAccessor.Get<double>(Id, nameof(DataFetchSize))!;
        set => PropertyAccessor.Set(Id, nameof(DataFetchSize), value);
    }

    public object? Footer
    {
        get => PropertyAccessor.Get<object>(Id, nameof(Footer));
        set
        {
            if (value is WinUIShellObject v)
            {
                PropertyAccessor.Set(Id, nameof(Footer), v.Id);
            }
            else
            {
                PropertyAccessor.Set(Id, nameof(Footer), value);
            }
        }
    }

    //public DataTemplate FooterTemplate
    //public TransitionCollection FooterTransitions

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
    //public TransitionCollection HeaderTransitions

    public double IncrementalLoadingThreshold
    {
        get => PropertyAccessor.Get<double>(Id, nameof(IncrementalLoadingThreshold))!;
        set => PropertyAccessor.Set(Id, nameof(IncrementalLoadingThreshold), value);
    }

    public IncrementalLoadingTrigger IncrementalLoadingTrigger
    {
        get => PropertyAccessor.Get<IncrementalLoadingTrigger>(Id, nameof(IncrementalLoadingTrigger))!;
        set => PropertyAccessor.Set(Id, nameof(IncrementalLoadingTrigger), value);
    }

    public bool IsActiveView
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsActiveView))!;
        set => PropertyAccessor.Set(Id, nameof(IsActiveView), value);
    }

    public bool IsItemClickEnabled
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsItemClickEnabled))!;
        set => PropertyAccessor.Set(Id, nameof(IsItemClickEnabled), value);
    }

    public bool IsMultiSelectCheckBoxEnabled
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsMultiSelectCheckBoxEnabled))!;
        set => PropertyAccessor.Set(Id, nameof(IsMultiSelectCheckBoxEnabled), value);
    }

    public bool IsSwipeEnabled
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsSwipeEnabled))!;
        set => PropertyAccessor.Set(Id, nameof(IsSwipeEnabled), value);
    }

    public bool IsZoomedInView
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsZoomedInView))!;
        set => PropertyAccessor.Set(Id, nameof(IsZoomedInView), value);
    }

    public ListViewReorderMode ReorderMode
    {
        get => PropertyAccessor.Get<ListViewReorderMode>(Id, nameof(ReorderMode))!;
        set => PropertyAccessor.Set(Id, nameof(ReorderMode), value);
    }

    public WinUIShellObjectList<object> SelectedItems
    {
        get => PropertyAccessor.Get<WinUIShellObjectList<object>>(Id, nameof(SelectedItems))!;
    }

    public WinUIShellObjectReadOnlyList<ItemIndexRange> SelectedRanges
    {
        get => PropertyAccessor.Get<WinUIShellObjectReadOnlyList<ItemIndexRange>>(Id, nameof(SelectedRanges))!;
    }

    public ListViewSelectionMode SelectionMode
    {
        get => PropertyAccessor.Get<ListViewSelectionMode>(Id, nameof(SelectionMode))!;
        set => PropertyAccessor.Set(Id, nameof(SelectionMode), value);
    }

    //public SemanticZoom SemanticZoomOwner

    public bool ShowsScrollingPlaceholders
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(ShowsScrollingPlaceholders))!;
        set => PropertyAccessor.Set(Id, nameof(ShowsScrollingPlaceholders), value);
    }

    public bool SingleSelectionFollowsFocus
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(SingleSelectionFollowsFocus))!;
        set => PropertyAccessor.Set(Id, nameof(SingleSelectionFollowsFocus), value);
    }

    internal ListViewBase()
    {
    }

    internal ListViewBase(ObjectId id)
        : base(id)
    {
    }

    public void ScrollIntoView(object item)
    {
        CommandClient.Get().InvokeMethod(
            Id,
            nameof(ScrollIntoView),
            item);
    }

    public void SelectAll()
    {
        CommandClient.Get().InvokeMethod(
            Id,
            nameof(SelectAll));
    }

    public void AddItemClick(ScriptBlock scriptBlock, object? argumentList = null)
    {
        AddItemClick(new EventCallback
        {
            ScriptBlock = scriptBlock,
            ArgumentList = argumentList
        });
    }
    public void AddItemClick(EventCallback eventCallback)
    {
        _callbacks.Add(
            Id,
            "ItemClick",
            ObjectTypeMapping.Get().GetTargetTypeName<ItemClickEventArgs>(),
            eventCallback);
    }
}
