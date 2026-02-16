namespace WinUIShell.Windows.Foundation;

public partial interface IAsyncActionWithProgress<TProgress> : IAsyncInfo
{
    void WaitForCompleted()
    {
        AsyncInfoMethods.WaitForCompleted(this);
    }
}
