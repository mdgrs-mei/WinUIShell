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

    internal Style(Resource resource)
        : base(resource.Id)
    {
    }

    public static implicit operator Style(Resource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new Style(resource.Id);
    }
}
