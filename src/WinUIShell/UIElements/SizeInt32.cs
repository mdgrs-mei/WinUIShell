using WinUIShell.Common;

namespace WinUIShell;

public class SizeInt32 : WinUIShellObject
{
    public int Width
    {
        get => PropertyAccessor.Get<int>(Id, nameof(Width))!;
    }

    public int Height
    {
        get => PropertyAccessor.Get<int>(Id, nameof(Width))!;
    }

    internal SizeInt32(ObjectId id)
        : base(id)
    {
    }

    public SizeInt32(int width, int height)
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<SizeInt32>(),
            this,
            width,
            height);
    }
}
