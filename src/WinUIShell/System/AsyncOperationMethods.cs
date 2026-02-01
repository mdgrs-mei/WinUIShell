using WinUIShell.Common;
using WinUIShell.Windows.Foundation;

namespace WinUIShell;

internal static class AsyncOperationMethods
{
    public static TResult WaitForCompleted<TResult>(IAsyncOperation<TResult> op)
    {
        while (true)
        {
            if (op.Status is
                AsyncStatus.Completed or
                AsyncStatus.Error or
                AsyncStatus.Canceled)
            {
                return op.GetResults();
            }

            Engine.Get().UpdateRunspace();
            Thread.Sleep(Constants.ClientCommandPolingIntervalMillisecond);
        }
    }
}
