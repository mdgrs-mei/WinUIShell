namespace WinUIShell.Windows.Foundation;

public partial interface IAsyncOperation<TResult> : IAsyncInfo
{
    TResult WaitForCompleted()
    {
        AsyncInfoMethods.WaitForCompleted(this);
        return GetResults();
    }
}
