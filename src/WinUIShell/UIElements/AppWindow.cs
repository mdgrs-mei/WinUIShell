using RpcUIShell.Core;

namespace WinUIShell.Microsoft.UI.Windowing;

public partial class AppWindow : IWinUIShellObject
{
    public void Resize(int width, int height)
    {
        var size = new WinUIShell.Windows.Graphics.SizeInt32(width, height);

        CommandClient.Get().InvokeMethod(
            WinUIShellObjectId,
            null,
            "Resize",
            size.WinUIShellObjectId);
    }

    public void ResizeClient(int width, int height)
    {
        var size = new WinUIShell.Windows.Graphics.SizeInt32(width, height);

        CommandClient.Get().InvokeMethod(
            WinUIShellObjectId,
            null,
            "ResizeClient",
            size.WinUIShellObjectId);
    }
}
