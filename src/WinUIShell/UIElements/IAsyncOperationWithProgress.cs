namespace WinUIShell.Windows.Foundation;

public partial interface IAsyncOperationWithProgress<TResult, TProgress> : IAsyncInfo
{
    TResult WaitForCompleted()
    {
        AsyncInfoMethods.WaitForCompleted(this);
        return GetResults();
    }
}
