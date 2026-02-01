namespace WinUIShell.Windows.Devices.Sms;

public partial class GetSmsDeviceOperation : Foundation.IAsyncOperation<SmsDevice>, Foundation.IAsyncInfo
{
    public SmsDevice WaitForCompleted()
    {
        return AsyncOperationMethods.WaitForCompleted(this);
    }
}
