using WinUIShell.Common;

namespace WinUIShell;

public static class XamlReader
{
    public static object? Load(string xaml)
    {
        return CommandClient.Get().InvokeStaticMethodAndGetResult<object>(
                    "Microsoft.UI.Xaml.Markup.XamlReader, Microsoft.WinUI",
                    nameof(Load),
                    xaml);
    }
}
