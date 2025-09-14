using WinUIShell.Common;

namespace WinUIShell;

public class KeyRoutedEventArgs : RoutedEventArgs
{
    public string DeviceId
    {
        get => PropertyAccessor.Get<string>(Id, nameof(DeviceId))!;
    }

    public bool Handled
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(Handled))!;
        set => PropertyAccessor.SetAndWait(Id, nameof(Handled), value);
    }

    public VirtualKey Key
    {
        get => PropertyAccessor.Get<VirtualKey>(Id, nameof(Key))!;
    }

    //public CorePhysicalKeyStatus KeyStatus => IKeyRoutedEventArgsMethods.get_KeyStatus(_objRef_global__Microsoft_UI_Xaml_Input_IKeyRoutedEventArgs);

    public VirtualKey OriginalKey
    {
        get => PropertyAccessor.Get<VirtualKey>(Id, nameof(OriginalKey))!;
    }

    internal KeyRoutedEventArgs(ObjectId id)
        : base(id)
    {
    }
}
