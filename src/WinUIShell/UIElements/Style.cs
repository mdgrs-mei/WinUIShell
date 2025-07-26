using WinUIShell.Common;

namespace WinUIShell;

public class Style : WinUIShellObject
{
    public Style BasedOn
    {
        get => PropertyAccessor.Get<Style>(Id, nameof(BasedOn))!;
    }

    internal Style(ObjectId id)
    : base(id)
    {
    }
}
