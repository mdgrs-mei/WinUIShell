using Microsoft.CodeAnalysis;
using RpcUIShell.Core;

namespace RpcUIShell.Generator;

internal static class ApiGenerator
{
    private static readonly Dictionary<string, ObjectDef> _objectDefs = [];

    public static void Generate(SourceProductionContext sourceProductionContext)
    {
        Api api = Generator.Api!;
        foreach (var apiEnumDef in api.Enums)
        {
            EnumDef enumDef = new(apiEnumDef);
            string filename = enumDef.GetSourceCodeFileName();
            string sourceCode = enumDef.Generate();
            sourceProductionContext.AddSource(filename, sourceCode);
        }

        CreateObjectDefMap();

        foreach (var keyValuePair in _objectDefs)
        {
            ObjectDef objectDef = keyValuePair.Value;
            string filename = objectDef.GetSourceCodeFileName();
            string sourceCode = objectDef.Generate();
            sourceProductionContext.AddSource(filename, sourceCode);
        }

        DestroyObjectDefMap();
    }

    private static void CreateObjectDefMap()
    {
        Api api = Generator.Api!;

        foreach (var apiObjectDef in api.Objects)
        {
            ObjectDef objectDef = new(apiObjectDef);
            if (!objectDef.IsSupported())
                continue;

            var typeId = objectDef.Type.GetId();
            if (_objectDefs.ContainsKey(typeId))
            {
                DestroyObjectDefMap();
                throw new InvalidOperationException($"Duplicate object found: {typeId}");
            }
            _objectDefs[typeId] = objectDef;
        }
    }

    private static void DestroyObjectDefMap()
    {
        _objectDefs.Clear();
    }

    public static ObjectDef? GetObjectDef(TypeDef typeDef)
    {
        var typeId = typeDef.GetId();
        if (_objectDefs.TryGetValue(typeId, out var objectDef))
        {
            return objectDef;
        }
        return null;
    }
}
