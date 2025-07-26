using WinUIShell.Common;

namespace WinUIShell;

public sealed class XamlReader
{
    public static object? Load(string xaml)
    {
        return CommandClient.Get().InvokeStaticMethodAndGetResult<object>(
            ObjectTypeMapping.Get().GetTargetTypeName<XamlReader>(),
            nameof(Load),
            xaml);
    }
}
