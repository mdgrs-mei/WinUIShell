using Microsoft.UI.Xaml;

namespace WinUIShell.Server;
internal sealed partial class Button : Microsoft.UI.Xaml.Controls.Button, IButton
{
    public void AddClick(
        int queueThreadId,
        int eventId,
        object?[]? disabledControlsWhileProcessing)
    {
        var callback = EventCallback.Create<RoutedEventArgs>(
            this,
            "OnClick",
            queueThreadId,
            eventId,
            disabledControlsWhileProcessing);
        Click += new RoutedEventHandler(callback);
    }
}
