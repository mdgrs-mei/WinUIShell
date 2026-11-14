using RpcUIShell.Core;

namespace WinUIShell;

internal static partial class TypeMappingInitializer
{
    private static bool s_isInitialized;

    public static void Init()
    {
        if (s_isInitialized)
            return;

        ObjectTypeMapping.Get().Init(
            ObjectTypeMapping.MappingDirection.ClientToServer,
            "WinUIShell");

        InitEnumTypeMapping();
        InitObjectTypeMapping();
        s_isInitialized = true;
    }
}
