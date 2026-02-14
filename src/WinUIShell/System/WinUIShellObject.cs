
using WinUIShell.Common;
namespace WinUIShell;

public sealed class WinUIShellObject : IWinUIShellObject
{
    public ObjectId WinUIShellObjectId { get; } = new();

    internal WinUIShellObject(ObjectId id)
    {
        WinUIShellObjectId = id;
    }
}
