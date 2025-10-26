using WinUIShell.Server;

namespace WinUIShell.Generator;

internal class ArgumentType
{
    private readonly string _name = "";
    private readonly Api.ArgumentType _apiArgumentType;
    private readonly ArgumentType? _elementType;
    private readonly List<ArgumentType>? _genericTypeArguments;
    public bool IsNullable { get; internal set; }
    public bool IsRpcSupportedType { get; internal set; }
    public bool IsSystemInterface { get; internal set; }
    public bool IsObject { get; internal set; }
    public bool IsVoid { get; internal set; }

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
        "System.IntPtr",
        "Microsoft.UI.Xaml.DependencyObject",
        "WinRT.IWinRTObject",
    ];

    private static readonly List<string> _supportedSystemInterfaces =
    [
        "System.Collections.Generic.ICollection",
        "System.Collections.Generic.IList",
        "System.Collections.Generic.IDictionary",
    ];

    public ArgumentType(Api.ArgumentType apiArgumentType)
    {
        _apiArgumentType = apiArgumentType;

        var serverTypeName = apiArgumentType.Name;
        IsRpcSupportedType = apiArgumentType.IsEnum;
        if (_apiArgumentType.IsInterface && serverTypeName.StartsWith("System."))
        {
            IsSystemInterface = true;
        }

        if (_apiArgumentType.IsGenericTypeParameter || _apiArgumentType.ElementType is not null)
        {
            _name = serverTypeName;
        }
        else
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

        IsNullable = apiArgumentType.IsNullable || !IsRpcSupportedType;

        if (apiArgumentType.ElementType is not null)
        {
            _elementType = new ArgumentType(apiArgumentType.ElementType);
        }
        if (apiArgumentType.GenericTypeArguments.Count > 0)
        {
            _genericTypeArguments = [];
            foreach (var genericTypeArgument in apiArgumentType.GenericTypeArguments)
            {
                _genericTypeArguments.Add(new ArgumentType(genericTypeArgument));
            }
        }
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

    public bool IsSupported()
    {
#if false
        if (_apiArgumentType.IsArray)
            return false;

        if (IsRefOrOut())
            return false;
#endif
        if (IsGenericParameter())
            return true;

        if (IsUnsupportedType())
            return false;

        if (IsUnsupportedSystemInterface())
            return false;

        return true;
    }

    private bool IsUnsupportedType()
    {
        return _unsupportedTypes.Contains(_apiArgumentType.Name) || _apiArgumentType.IsDelegate;
    }

    private bool IsUnsupportedSystemInterface()
    {
        return IsSystemInterface && !_supportedSystemInterfaces.Contains(_apiArgumentType.Name);
    }

    private bool IsRefOrOut()
    {
        return _apiArgumentType.IsOut || (_apiArgumentType.IsByRef && !_apiArgumentType.IsIn);
    }

    private bool IsGenericParameter()
    {
        return _apiArgumentType.IsGenericTypeParameter || _apiArgumentType.IsGenericMethodParameter;
    }

    public string GetName()
    {
        if (_elementType is not null)
        {
            if (_apiArgumentType.IsArray)
            {
                return $"{_elementType.GetName()}[]";
            }
            return _elementType.GetName();
        }

        if (_genericTypeArguments is not null)
        {
            var genericTypeArgumentsNames = _genericTypeArguments.Select(t => t.GetName());
            return $"{_name}<{string.Join(", ", genericTypeArgumentsNames)}>";
        }

        return _name;
    }

    public string GetTypeExpression()
    {
        string refExpression = "";
        if (_apiArgumentType.IsByRef)
        {
            if (_apiArgumentType.IsIn)
            {
                refExpression = "in ";
            }
            else
            if (_apiArgumentType.IsOut)
            {
                refExpression = "out ";
            }
            else
            {
                refExpression = "ref ";
            }
        }

        return $"{refExpression}{GetName()}{(IsNullable ? "?" : "")}";
    }

    public string GetValueExpression()
    {
        if (IsRpcSupportedType)
        {
            return "value";
        }
        else
        if (IsObject || IsGenericParameter())
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
        if (_apiArgumentType.IsByRef)
        {
            return _elementType!.GetArgumentExpression(variableName);
        }
        else
        if (IsRpcSupportedType)
        {
            return variableName;
        }
        else
        if (IsObject || IsGenericParameter())
        {
            return $"({variableName} is WinUIShellObject v ? v.Id : {variableName})";
        }
        else
        {
            return $"{variableName}?.Id";
        }
    }
}
