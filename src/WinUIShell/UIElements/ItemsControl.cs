using WinUIShell.Common;

namespace WinUIShell;

public class ItemsControl : Control
{
    public string DisplayMemberPath
    {
        get => PropertyAccessor.Get<string>(Id, nameof(DisplayMemberPath))!;
        set => PropertyAccessor.Set(Id, nameof(DisplayMemberPath), value);
    }

    //public IObservableVector<GroupStyle> GroupStyle => IItemsControlMethods.get_GroupStyle(_objRef_global__Microsoft_UI_Xaml_Controls_IItemsControl);
    //public GroupStyleSelector GroupStyleSelector

    public bool IsGrouping
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsGrouping))!;
    }

    //public ItemContainerGenerator ItemContainerGenerator => IItemsControlMethods.get_ItemContainerGenerator(_objRef_global__Microsoft_UI_Xaml_Controls_IItemsControl);

    public Style? ItemContainerStyle
    {
        get => PropertyAccessor.Get<Style>(Id, nameof(ItemContainerStyle));
        set => PropertyAccessor.Set(Id, nameof(ItemContainerStyle), value?.Id);
    }

    //public StyleSelector ItemContainerStyleSelector
    //public TransitionCollection ItemContainerTransitions
    //public DataTemplate ItemTemplate
    //public DataTemplateSelector ItemTemplateSelector

    public ItemCollection Items
    {
        get => PropertyAccessor.Get<ItemCollection>(Id, nameof(Items))!;
    }

    //public ItemsPanelTemplate ItemsPanel

    public Panel? ItemsPanelRoot
    {
        get => PropertyAccessor.Get<Panel>(Id, nameof(ItemsPanelRoot));
        set => PropertyAccessor.Set(Id, nameof(ItemsPanelRoot), value?.Id);
    }

    //public object ItemsSource

    internal ItemsControl()
    {
    }

    internal ItemsControl(ObjectId id)
        : base(id)
    {
    }
}
