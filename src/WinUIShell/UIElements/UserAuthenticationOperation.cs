namespace WinUIShell.Windows.Security.Authentication.OnlineId;

public partial class UserAuthenticationOperation : Foundation.IAsyncOperation<UserIdentity>, Foundation.IAsyncInfo
{
    public UserIdentity WaitForCompleted()
    {
        return AsyncOperationMethods.WaitForCompleted(this);
    }
}
