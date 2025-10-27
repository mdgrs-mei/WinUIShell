using System.Xml.Serialization;
using Microsoft.CodeAnalysis;
using WinUIShell.Server;

namespace WinUIShell.Generator;

[Generator(LanguageNames.CSharp)]
public class Generator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput((postInitContext) =>
        {

        });

        var provider = context.AdditionalTextsProvider.Where((text) =>
        {
            return text.Path.EndsWith("Api.xml");
        }).Combine(context.AnalyzerConfigOptionsProvider);

        context.RegisterSourceOutput(provider, (sourceProductionContext, source) =>
        {
            var text = source.Left.GetText();
            if (text is null)
                return;

            var api = LoadApi(text.ToString());
            if (api is null)
                return;

            var configOptionsProvider = source.Right;
            if (configOptionsProvider.GlobalOptions.TryGetValue("build_property.WinUIShellGenerator_GenerateTypeMapping", out var generateTypeMapping))
            {
                EnumGenerator.GenerateTypeMapping(sourceProductionContext, api);
            }
            else
            {
                EnumGenerator.Generate(sourceProductionContext, api);
                ObjectGenerator.Generate(sourceProductionContext, api);
            }
        });
    }

    private static Api LoadApi(string content)
    {
        var stringReader = new StringReader(content);
        var serializer = new XmlSerializer(typeof(Api));
        var api = (Api)serializer.Deserialize(stringReader);
        return api;
    }

    internal static string GetTargetNamespace(string serverNamespace)
    {
        return serverNamespace == "WinUIShell.Server" ? "WinUIShell" : $"WinUIShell.{serverNamespace}";
    }
}
