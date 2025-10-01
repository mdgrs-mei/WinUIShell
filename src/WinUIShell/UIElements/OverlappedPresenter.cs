using WinUIShell.Common;

namespace WinUIShell;

public class OverlappedPresenter : AppWindowPresenter
{
    public bool HasBorder
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(HasBorder))!;
    }

    public bool HasTitleBar
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(HasTitleBar))!;
    }

    public bool IsAlwaysOnTop
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsAlwaysOnTop))!;
        set => PropertyAccessor.Set(Id, nameof(IsAlwaysOnTop), value);
    }

    public bool IsMaximizable
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsMaximizable))!;
        set => PropertyAccessor.Set(Id, nameof(IsMaximizable), value);
    }

    public bool IsMinimizable
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsMinimizable))!;
        set => PropertyAccessor.Set(Id, nameof(IsMinimizable), value);
    }

    public bool IsModal
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsModal))!;
        set => PropertyAccessor.Set(Id, nameof(IsModal), value);
    }

    public bool IsResizable
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsResizable))!;
        set => PropertyAccessor.Set(Id, nameof(IsResizable), value);
    }

    public int? PreferredMaximumHeight
    {
        get => PropertyAccessor.Get<int>(Id, nameof(PreferredMaximumHeight));
        set => PropertyAccessor.Set(Id, nameof(PreferredMaximumHeight), value);
    }

    public int? PreferredMaximumWidth
    {
        get => PropertyAccessor.Get<int>(Id, nameof(PreferredMaximumWidth));
        set => PropertyAccessor.Set(Id, nameof(PreferredMaximumWidth), value);
    }

    public int? PreferredMinimumHeight
    {
        get => PropertyAccessor.Get<int>(Id, nameof(PreferredMinimumHeight));
        set => PropertyAccessor.Set(Id, nameof(PreferredMinimumHeight), value);
    }

    public int? PreferredMinimumWidth
    {
        get => PropertyAccessor.Get<int>(Id, nameof(PreferredMinimumWidth));
        set => PropertyAccessor.Set(Id, nameof(PreferredMinimumWidth), value);
    }

    public Microsoft.UI.Windowing.OverlappedPresenterState State
    {
        get => PropertyAccessor.Get<Microsoft.UI.Windowing.OverlappedPresenterState>(Id, nameof(State))!;
    }

    public OverlappedPresenter()
    {
        Id = CommandClient.Get().CreateObjectWithStaticMethod(
            ObjectTypeMapping.Get().GetTargetTypeName<OverlappedPresenter>(),
            "Create",
            this);
    }

    internal OverlappedPresenter(ObjectId id)
        : base(id)
    {
    }
}
