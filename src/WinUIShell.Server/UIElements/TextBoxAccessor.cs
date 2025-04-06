using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace WinUIShell.Server;
internal static class TextBoxAccessor
{
    public static void AddBeforeTextChanging(
        TextBox target,
        int queueThreadId,
        int eventId,
        object?[]? disabledControlsWhileProcessing)
    {
        var callback = EventCallback.Create<TextBoxBeforeTextChangingEventArgs>(
            target,
            "OnBeforeTextChanging",
            queueThreadId,
            eventId,
            disabledControlsWhileProcessing);

        target.BeforeTextChanging += new TypedEventHandler<TextBox, TextBoxBeforeTextChangingEventArgs>(callback);
    }

    public static void AddTextChanged(
        TextBox target,
        int queueThreadId,
        int eventId,
        object?[]? disabledControlsWhileProcessing)
    {
        var callback = EventCallback.Create<TextChangedEventArgs>(
            target,
            "OnTextChanged",
            queueThreadId,
            eventId,
            disabledControlsWhileProcessing);

        target.TextChanged += new TextChangedEventHandler(callback);
    }
}
