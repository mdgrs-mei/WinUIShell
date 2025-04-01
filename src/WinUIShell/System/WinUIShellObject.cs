
using WinUIShell.Common;
namespace WinUIShell;

public class WinUIShellObject
{
    public ObjectId Id { get; protected set; } = new();

    internal WinUIShellObject()
    {
    }

    internal WinUIShellObject(ObjectId id)
    {
        Id = id;
    }
}
