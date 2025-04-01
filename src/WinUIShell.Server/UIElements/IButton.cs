namespace WinUIShell.Server;
internal interface IButton
{
    void AddClick(
        int queueThreadId,
        int eventId,
        object?[]? disabledControlsWhileProcessing);
}
