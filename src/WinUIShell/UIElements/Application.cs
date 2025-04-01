using WinUIShell.Common;

namespace WinUIShell;

public class Application : WinUIShellObject
{
    public static Application Current
    {
        get => PropertyAccessor.GetStatic<Application>(
            "Microsoft.UI.Xaml.Application, Microsoft.WinUI",
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
