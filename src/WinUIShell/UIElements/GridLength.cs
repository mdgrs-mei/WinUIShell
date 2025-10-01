using WinUIShell.Common;

namespace WinUIShell;

public class GridLength : WinUIShellObject
{
    public double Value
    {
        get => PropertyAccessor.Get<double>(Id, nameof(Value))!;
    }

    public Microsoft.UI.Xaml.GridUnitType GridUnitType
    {
        get => PropertyAccessor.Get<Microsoft.UI.Xaml.GridUnitType>(Id, nameof(GridUnitType))!;
    }

    public bool IsAbsolute
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsAbsolute))!;
    }

    public bool IsAuto
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsAuto))!;
    }

    public bool IsStar
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsStar))!;
    }

    public static GridLength Auto
    {
        get => PropertyAccessor.GetStatic<GridLength>(
            ObjectTypeMapping.Get().GetTargetTypeName<GridLength>(),
            nameof(Auto))!;
    }

    public GridLength(double pixels)
        : this(pixels, Microsoft.UI.Xaml.GridUnitType.Pixel)
    {
    }

    public GridLength(double value, Microsoft.UI.Xaml.GridUnitType type)
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<GridLength>(),
            this,
            value,
            type);
    }

    internal GridLength(ObjectId id)
    : base(id)
    {
    }

    public override string ToString()
    {
        return CommandClient.Get().InvokeMethodAndGetResult<string>(Id, nameof(ToString))!;
    }
}
