using WinUIShell.Common;

namespace WinUIShell;

public class Color : WinUIShellObject
{
    public byte A
    {
        get => PropertyAccessor.Get<byte>(Id, nameof(A))!;
    }

    public byte R
    {
        get => PropertyAccessor.Get<byte>(Id, nameof(R))!;
    }

    public byte G
    {
        get => PropertyAccessor.Get<byte>(Id, nameof(G))!;
    }

    public byte B
    {
        get => PropertyAccessor.Get<byte>(Id, nameof(B))!;
    }

    internal Color(ObjectId id)
        : base(id)
    {
    }

    public static Color FromArgb(byte a, byte r, byte g, byte b)
    {
        return CommandClient.Get().InvokeStaticMethodAndGetResult<Color>(
            "Windows.UI.Color, Microsoft.Windows.SDK.NET",
            nameof(FromArgb),
            a, r, g, b)!;
    }

    public override string ToString()
    {
        return CommandClient.Get().InvokeMethodAndGetResult<string>(Id, nameof(ToString))!;
    }
}
