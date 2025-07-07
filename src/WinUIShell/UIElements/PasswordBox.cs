using WinUIShell.Common;

namespace WinUIShell;

public class PasswordBox : Control
{
    public object? Description
    {
        get => PropertyAccessor.Get<object>(Id, nameof(Description));
        set
        {
            if (value is WinUIShellObject v)
            {
                PropertyAccessor.Set(Id, nameof(Description), v.Id);
            }
            else
            {
                PropertyAccessor.Set(Id, nameof(Description), value);
            }
        }
    }

    public object? Header
    {
        get => PropertyAccessor.Get<object>(Id, nameof(Header));
        set
        {
            if (value is WinUIShellObject v)
            {
                PropertyAccessor.Set(Id, nameof(Header), v.Id);
            }
            else
            {
                PropertyAccessor.Set(Id, nameof(Header), value);
            }
        }
    }

    public bool IsPasswordRevealButtonEnabled
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsPasswordRevealButtonEnabled))!;
        set => PropertyAccessor.Set(Id, nameof(IsPasswordRevealButtonEnabled), value);
    }

    public int MaxLength
    {
        get => PropertyAccessor.Get<int>(Id, nameof(MaxLength))!;
        set => PropertyAccessor.Set(Id, nameof(MaxLength), value);
    }

    public string Password
    {
        get => PropertyAccessor.Get<string>(Id, nameof(Password))!;
        set => PropertyAccessor.Set(Id, nameof(Password), value);
    }

    public string PasswordChar
    {
        get => PropertyAccessor.Get<string>(Id, nameof(PasswordChar))!;
        set => PropertyAccessor.Set(Id, nameof(PasswordChar), value);
    }

    public string PlaceholderText
    {
        get => PropertyAccessor.Get<string>(Id, nameof(PlaceholderText))!;
        set => PropertyAccessor.Set(Id, nameof(PlaceholderText), value);
    }

    public PasswordBox()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<PasswordBox>(),
            this);
    }
}
