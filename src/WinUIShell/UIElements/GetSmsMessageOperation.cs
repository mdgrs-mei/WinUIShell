namespace WinUIShell.Windows.Devices.Sms;

public partial class GetSmsMessageOperation : Foundation.IAsyncOperation<ISmsMessage>, Foundation.IAsyncInfo
{
    public ISmsMessage WaitForCompleted()
    {
        return AsyncOperationMethods.WaitForCompleted(this);
    }
}
