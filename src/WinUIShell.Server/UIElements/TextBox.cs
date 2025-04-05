using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace WinUIShell.Server;
internal sealed partial class TextBox : Microsoft.UI.Xaml.Controls.TextBox
{
    public void AddBeforeTextChanging(
        int queueThreadId,
        int eventId,
        object?[]? disabledControlsWhileProcessing)
    {
        var callback = EventCallback.Create<TextBoxBeforeTextChangingEventArgs>(
            this,
            "OnBeforeTextChanging",
            queueThreadId,
            eventId,
            disabledControlsWhileProcessing);

        BeforeTextChanging += new TypedEventHandler<Microsoft.UI.Xaml.Controls.TextBox, TextBoxBeforeTextChangingEventArgs>(callback);
    }

    public void AddTextChanged(
        int queueThreadId,
        int eventId,
        object?[]? disabledControlsWhileProcessing)
    {
        var callback = EventCallback.Create<TextChangedEventArgs>(
            this,
            "OnTextChanged",
            queueThreadId,
            eventId,
            disabledControlsWhileProcessing);

        TextChanged += new TextChangedEventHandler(callback);
    }
}
