namespace WinUIShell;

public abstract class ContentControl : Control
{
    public object? Content
    {
        get => PropertyAccessor.Get<object>(Id, nameof(Content));
        set
        {
            if (value is WinUIShellObject v)
            {
                PropertyAccessor.Set(Id, nameof(Content), v.Id);
            }
            else
            {
                PropertyAccessor.Set(Id, nameof(Content), value);
            }
        }
    }
}
