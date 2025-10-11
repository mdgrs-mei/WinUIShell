using WinUIShell.Server;

namespace WinUIShell.Generator;

internal class ArgumentType
{
    public string Name { get; internal set; } = "";
    public bool IsNullable { get; internal set; }
    public bool IsPrimitiveType { get; internal set; }
    public bool IsArray { get; internal set; }
    public bool IsObject { get; internal set; }
    public bool IsSupported { get; internal set; } = true;

    private static readonly List<(string FullName, string ShortName)> _systemTypes =
    [
        ("System.Boolean", "bool"),
        ("System.Byte", "byte"),
        ("System.SByte", "sbyte"),
        ("System.Char", "char"),
        ("System.Decimal", "decimal"),
        ("System.Double", "double"),
        ("System.Single", "float"),
        ("System.Int32", "int"),
        ("System.UInt32", "uint"),
        ("System.Int64", "long"),
        ("System.UInt64", "ulong"),
        ("System.Int16", "short"),
        ("System.UInt16", "ushort"),
        ("System.String", "string"),
        ("System.Void", "void")
    ];

    private static readonly List<string> _unsupportedTypes =
    [
        "System.Collections.Generic.ICollection",
        "System.Collections.Generic.IList",
        "System.Collections.Generic.IDictionary",
    ];

    public ArgumentType(Api.ArgumentType apiArgumentType)
    {
        var serverTypeName = apiArgumentType.Name;
        IsPrimitiveType = apiArgumentType.IsValueType;
        IsArray = apiArgumentType.IsArray;

        if (serverTypeName.StartsWith("WinUIShell.Server"))
        {
            Name = serverTypeName.Replace("WinUIShell.Server", "WinUIShell");
        }
        else
        if (serverTypeName == "System.Object")
        {
            Name = "object";
            IsObject = true;
        }
        else
        if (TryReplaceSystemType(serverTypeName, out var systemTypeName))
        {
            Name = systemTypeName!;
            IsPrimitiveType = true;
        }
        else
        {
            Name = $"WinUIShell.{serverTypeName}";
            IsSupported = IsSupportedType(serverTypeName);
        }

        IsNullable = IsNullable || !IsPrimitiveType;
    }

    private static bool TryReplaceSystemType(string typeName, out string? systemTypeName)
    {
        foreach (var (FullName, ShortName) in _systemTypes)
        {
            if (typeName.Contains(FullName))
            {
                systemTypeName = typeName.Replace(FullName, ShortName);
                return true;
            }
        }
        systemTypeName=null;
        return false;
    }

    private static bool IsSupportedType(string typeName)
    {
        foreach (var unsupportedType in _unsupportedTypes)
        {
            if (typeName.StartsWith(unsupportedType))
            {
                return false;
            }
        }
        return true;
    }
}
