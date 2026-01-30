using WinUIShell.Common;

namespace WinUIShell.Windows.Foundation;

public partial interface IAsyncOperation<TResult> : IAsyncInfo
{
    TResult WaitForCompleted();
}

public partial class IAsyncOperationImpl<TResult> : IAsyncOperation<TResult>
{
    public TResult WaitForCompleted()
    {
        while (true)
        {
            if (Status is
                AsyncStatus.Completed or
                AsyncStatus.Error or
                AsyncStatus.Canceled)
            {
                return GetResults();
            }

            Engine.Get().UpdateRunspace();
            Thread.Sleep(Constants.ClientCommandPolingIntervalMillisecond);
        }
    }
}
