using Microsoft.UI.Xaml;

namespace WinUIShell.ApiExporter;

#pragma warning disable CA1515 // Consider making public types internal
public partial class App : Application
{
#pragma warning restore CA1515
    public App()
    {
        Export();
    }

    private void Export()
    {
        string[] arguments = Environment.GetCommandLineArgs();
        if (arguments.Length != 2)
        {
            throw new ArgumentException("Specify a path to the output Api.xml file.");
        }

        string apiFilePath = arguments[1];
        var exporter = new Exporter();
        exporter.Export(apiFilePath);
        Exit();
    }
}
