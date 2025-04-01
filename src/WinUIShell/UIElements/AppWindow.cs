using WinUIShell.Common;

namespace WinUIShell;

public class AppWindow : WinUIShellObject
{
    public AppWindowTitleBar? TitleBar
    {
        get => PropertyAccessor.Get<AppWindowTitleBar>(Id, nameof(TitleBar));
    }

    public void Resize(int width, int height)
    {
        var sizeId = CommandClient.Get().CreateObject(
                "Windows.Graphics.SizeInt32, Microsoft.Windows.SDK.NET",
                null,
                width,
                height);

        CommandClient.Get().InvokeMethod(Id, "Resize", sizeId);

        CommandClient.Get().DestroyObject(sizeId);
    }

    public void ResizeClient(int width, int height)
    {
        var sizeId = CommandClient.Get().CreateObject(
                "Windows.Graphics.SizeInt32, Microsoft.Windows.SDK.NET",
                null,
                width,
                height);

        CommandClient.Get().InvokeMethod(Id, "ResizeClient", sizeId);

        CommandClient.Get().DestroyObject(sizeId);
    }

    public void SetPresenter(AppWindowPresenter presenter)
    {
        CommandClient.Get().InvokeMethod(Id, "SetPresenter", presenter?.Id);
    }

    internal AppWindow(ObjectId id)
        : base(id)
    {
    }
}
