using WinUIShell.Common;

namespace WinUIShell;

public class Application : WinUIShellObject
{
    public static Application Current
    {
        get => PropertyAccessor.GetStatic<Application>(
            ObjectTypeMapping.Get().GetTargetTypeName<Application>(),
            nameof(Current))!;
    }

    public ResourceDictionary Resources
    {
        get => PropertyAccessor.Get<ResourceDictionary>(Id, nameof(Resources))!;
    }

    internal Application(ObjectId id)
        : base(id)
    {
    }
}
