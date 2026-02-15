namespace WinUIShell.Windows.Foundation;

public partial interface IAsyncAction : IAsyncInfo
{
    void WaitForCompleted()
    {
        AsyncInfoMethods.WaitForCompleted(this);
    }
}
