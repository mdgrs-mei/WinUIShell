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
        var size = new SizeInt32(width, height);

        CommandClient.Get().InvokeMethod(Id, "Resize", size.Id);

        CommandClient.Get().DestroyObject(size.Id);
    }

    public void ResizeClient(int width, int height)
    {
        var size = new SizeInt32(width, height);

        CommandClient.Get().InvokeMethod(Id, "ResizeClient", size.Id);

        CommandClient.Get().DestroyObject(size.Id);
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
