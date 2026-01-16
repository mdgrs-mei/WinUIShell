using System.Xml.Serialization;
using Microsoft.CodeAnalysis;
using WinUIShell.Server;

namespace WinUIShell.Generator;

[Generator(LanguageNames.CSharp)]
public class Generator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(AttributeGenerator.Generate);

        var additionalTextsProvider = context.AdditionalTextsProvider.Where((text) =>
        {
            return text.Path.EndsWith("Api.xml");
        }).Collect();

        var attributesProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
            AttributeGenerator.SurpressByNameAttributeFullName,
            (syntaxNode, cancellationToken) => true,
            (generatorAttributeSyntaxContext, cancellationToken) => generatorAttributeSyntaxContext).Collect();

        var provider = additionalTextsProvider.Combine(context.AnalyzerConfigOptionsProvider).Combine(attributesProvider);

        context.RegisterSourceOutput(provider, (sourceProductionContext, providers) =>
        {
            var apiText = providers.Left.Left.FirstOrDefault()?.GetText();
            if (apiText is null)
                return;

            var api = LoadApi(apiText.ToString());
            if (api is null)
                return;

            var configOptionsProvider = providers.Left.Right;
            if (configOptionsProvider.GlobalOptions.TryGetValue("build_property.WinUIShellGenerator_GenerateTypeMapping", out var generateTypeMapping))
            {
                EnumGenerator.GenerateTypeMapping(sourceProductionContext, api);
                ObjectGenerator.GenerateTypeMapping(sourceProductionContext, api);
            }
            else
            {
                var surpressByNameAttributes = providers.Right;
                AttributeGenerator.InitSurpressDictionary(surpressByNameAttributes);

                EnumGenerator.Generate(sourceProductionContext, api);
                ObjectGenerator.Generate(sourceProductionContext, api);

                AttributeGenerator.TermSurpressDictionary();
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
