using WinUIShell.Common;

namespace WinUIShell.Microsoft.UI.Windowing;

public partial class AppWindow : IWinUIShellObject
{
    public void Resize(int width, int height)
    {
        var size = new WinUIShell.Windows.Graphics.SizeInt32(width, height);

        CommandClient.Get().InvokeMethod(WinUIShellObjectId, "Resize", size.WinUIShellObjectId);

        CommandClient.Get().DestroyObject(size.WinUIShellObjectId);
    }

    public void ResizeClient(int width, int height)
    {
        var size = new WinUIShell.Windows.Graphics.SizeInt32(width, height);

        CommandClient.Get().InvokeMethod(WinUIShellObjectId, "ResizeClient", size.WinUIShellObjectId);

        CommandClient.Get().DestroyObject(size.WinUIShellObjectId);
    }
}
