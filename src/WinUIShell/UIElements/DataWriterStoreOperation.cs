namespace WinUIShell.Windows.Storage.Streams;

public partial class DataWriterStoreOperation : Foundation.IAsyncOperation<uint>, Foundation.IAsyncInfo
{
    public uint WaitForCompleted()
    {
        return AsyncOperationMethods.WaitForCompleted(this);
    }
}
