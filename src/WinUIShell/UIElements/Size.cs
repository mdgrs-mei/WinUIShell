using WinUIShell.Common;

namespace WinUIShell;

public class Size : WinUIShellObject
{
    public double Width
    {
        get => PropertyAccessor.Get<double>(Id, nameof(Width))!;
    }

    public double Height
    {
        get => PropertyAccessor.Get<double>(Id, nameof(Height))!;
    }

    public Size(float width, float height)
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<Size>(),
            this,
            width,
            height);
    }

    public Size(double width, double height)
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<Size>(),
            this,
            width,
            height);
    }

    internal Size(ObjectId id)
        : base(id)
    {
    }

    public override string ToString()
    {
        return CommandClient.Get().InvokeMethodAndGetResult<string>(Id, nameof(ToString))!;
    }
}
