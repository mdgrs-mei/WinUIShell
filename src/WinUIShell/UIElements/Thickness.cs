using WinUIShell.Common;

namespace WinUIShell;

public class Thickness : WinUIShellObject
{
    public double Bottom
    {
        get => PropertyAccessor.Get<double>(Id, nameof(Bottom))!;
    }

    public double Left
    {
        get => PropertyAccessor.Get<double>(Id, nameof(Left))!;
    }

    public double Right
    {
        get => PropertyAccessor.Get<double>(Id, nameof(Right))!;
    }

    public double Top
    {
        get => PropertyAccessor.Get<double>(Id, nameof(Top))!;
    }

    public Thickness()
    {
        Id = CommandClient.Get().CreateObject(
            "Microsoft.UI.Xaml.Thickness, Microsoft.WinUI",
            this);
    }

    internal Thickness(ObjectId id)
        : base(id)
    {
    }

    public Thickness(double uniformLength)
        : this(uniformLength, uniformLength, uniformLength, uniformLength)
    {
    }

    public Thickness(double left, double top, double right, double bottom)
    {
        Id = CommandClient.Get().CreateObject(
            "Microsoft.UI.Xaml.Thickness, Microsoft.WinUI",
            this,
            left,
            top,
            right,
            bottom);
    }

    public override string ToString()
    {
        return CommandClient.Get().InvokeMethodAndGetResult<string>(Id, nameof(ToString))!;
    }
}
