namespace WinUIShell.Windows.Foundation;

public partial interface IAsyncOperation<TResult> : IAsyncInfo
{
    TResult WaitForCompleted()
    {
        return AsyncOperationMethods.WaitForCompleted(this);
    }
}
