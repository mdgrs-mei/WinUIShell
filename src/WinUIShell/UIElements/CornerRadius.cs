using WinUIShell.Common;

namespace WinUIShell;

public class CornerRadius : WinUIShellObject
{
    public double TopLeft
    {
        get => PropertyAccessor.Get<double>(Id, nameof(TopLeft))!;
    }

    public double TopRight
    {
        get => PropertyAccessor.Get<double>(Id, nameof(TopRight))!;
    }

    public double BottomRight
    {
        get => PropertyAccessor.Get<double>(Id, nameof(BottomRight))!;
    }

    public double BottomLeft
    {
        get => PropertyAccessor.Get<double>(Id, nameof(BottomLeft))!;
    }

    internal CornerRadius(ObjectId id)
        : base(id)
    {
    }

    public CornerRadius(double uniformRadius)
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<CornerRadius>(),
            this,
            uniformRadius);
    }

    public CornerRadius(double topLeft, double topRight, double bottomRight, double bottomLeft)
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<CornerRadius>(),
            this,
            topLeft,
            topRight,
            bottomRight,
            bottomLeft);
    }

    public override string ToString()
    {
        return CommandClient.Get().InvokeMethodAndGetResult<string>(Id, nameof(ToString))!;
    }
}
