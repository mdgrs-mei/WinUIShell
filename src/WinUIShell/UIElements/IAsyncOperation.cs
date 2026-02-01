namespace WinUIShell.Windows.Foundation;

public partial interface IAsyncOperation<TResult> : IAsyncInfo
{
    TResult WaitForCompleted();
}

public partial class IAsyncOperationImpl<TResult> : IAsyncOperation<TResult>
{
    public TResult WaitForCompleted()
    {
        return AsyncOperationMethods.WaitForCompleted(this);
    }
}
