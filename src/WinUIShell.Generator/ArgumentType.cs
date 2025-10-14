using WinUIShell.Server;

namespace WinUIShell.Generator;

internal class ArgumentType
{
    private readonly string _name = "";
    public bool IsNullable { get; internal set; }
    public bool IsRpcSupportedType { get; internal set; }
    public bool IsArray { get; internal set; }
    public bool IsObject { get; internal set; }
    public bool IsVoid { get; internal set; }
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
    ];

    private static readonly List<string> _unsupportedTypes =
    [
        "System.Collections.Generic.ICollection",
        "System.Collections.Generic.IList",
        "System.Collections.Generic.IDictionary",
        "System.IntPtr",
    ];

    public ArgumentType(Api.ArgumentType apiArgumentType)
    {
        var serverTypeName = apiArgumentType.Name;
        IsRpcSupportedType = apiArgumentType.IsEnum;
        IsArray = apiArgumentType.IsArray;

        if (serverTypeName.StartsWith("WinUIShell.Server"))
        {
            _name = serverTypeName.Replace("WinUIShell.Server", "WinUIShell");
        }
        else
        if (serverTypeName == "System.Object")
        {
            _name = "object";
            IsObject = true;
        }
        else
        if (serverTypeName == "System.Void")
        {
            _name = "void";
            IsVoid = true;
            IsRpcSupportedType = true;
        }
        else
        if (TryReplaceSystemType(serverTypeName, out var systemTypeName))
        {
            _name = systemTypeName!;
            IsRpcSupportedType = true;
        }
        else
        {
            _name = $"WinUIShell.{serverTypeName}";
        }

        IsNullable = IsNullable || !IsRpcSupportedType;
        IsSupported = IsSupportedType(serverTypeName) && !apiArgumentType.IsDelegate;
    }

    private static bool TryReplaceSystemType(string typeName, out string? systemTypeName)
    {
        foreach (var (FullName, ShortName) in _systemTypes)
        {
            if (typeName == FullName)
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

    public string GetName()
    {
        return _name;
    }

    public string GetTypeExpression()
    {
        return $"{_name}{(IsNullable ? "?" : "")}";
    }

    public string GetValueExpression()
    {
        if (IsRpcSupportedType)
        {
            return "value";
        }
        else
        if (IsObject)
        {
            return "(value is WinUIShellObject v ? v.Id : value)";
        }
        else
        {
            return "value?.Id";
        }
    }

    public string GetArgumentExpression(string variableName)
    {
        if (IsRpcSupportedType)
        {
            return variableName;
        }
        else
        if (IsObject)
        {
            return $"({variableName} is WinUIShellObject v ? v.Id : {variableName})";
        }
        else
        {
            return $"{variableName}?.Id";
        }
    }
}
