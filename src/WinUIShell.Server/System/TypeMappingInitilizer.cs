using RpcUIShell.Core;

namespace WinUIShell.Server;

internal static partial class TypeMappingInitializer
{
    public static void Init()
    {
        TypeMapping.Get().Init(
            TypeMapping.MappingDirection.ServerToClient,
            "WinUIShell");

        InitMapping();
    }
}
