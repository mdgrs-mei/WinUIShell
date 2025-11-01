using Microsoft.CodeAnalysis;
using WinUIShell.Server;

namespace WinUIShell.Generator;

internal static class ObjectGenerator
{
    public static void Generate(SourceProductionContext sourceProductionContext, Api api)
    {
        foreach (var apiObjectDef in api.Objects)
        {
            ObjectDef objectDef = new(apiObjectDef);
            if (!objectDef.IsSupported())
                continue;

            string filename = objectDef.GetSourceCodeFileName();
            string sourceCode = objectDef.Generate();
            sourceProductionContext.AddSource(filename, sourceCode);
        }
    }
}
