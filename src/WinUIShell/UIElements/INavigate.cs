using WinUIShell.Generator;
namespace WinUIShell.Microsoft.UI.Xaml.Controls;

public partial interface INavigate : IWinUIShellObject
{
    [SurpressGeneratorByName]
    void Navigate() { }
}
