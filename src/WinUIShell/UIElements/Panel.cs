using WinUIShell.Common;

namespace WinUIShell;

public class Panel : FrameworkElement
{
    public Brush Background
    {
        get => PropertyAccessor.Get<Brush>(Id, nameof(Background))!;
        set => PropertyAccessor.Set(Id, nameof(Background), value?.Id);
    }

    //public BrushTransition BackgroundTransition
    //public TransitionCollection ChildrenTransitions
    //public bool IsItemsHost => IPanelMethods.get_IsItemsHost(_objRef_global__Microsoft_UI_Xaml_Controls_IPanel);

    public UIElementCollection Children
    {
        get => PropertyAccessor.Get<UIElementCollection>(Id, nameof(Children))!;
    }

    internal Panel()
    {
    }

    internal Panel(ObjectId id)
        : base(id)
    {
    }
}
