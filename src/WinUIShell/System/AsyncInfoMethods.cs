using WinUIShell.Common;
namespace WinUIShell.Windows.Foundation;

internal static class AsyncInfoMethods
{
    public static void WaitForCompleted(IAsyncInfo asyncInfo)
    {
        while (true)
        {
            if (asyncInfo.Status is
                AsyncStatus.Completed or
                AsyncStatus.Error or
                AsyncStatus.Canceled)
            {
                return;
            }

            Engine.Get().UpdateRunspace();
            Thread.Sleep(Constants.ClientCommandPolingIntervalMillisecond);
        }
    }
}
