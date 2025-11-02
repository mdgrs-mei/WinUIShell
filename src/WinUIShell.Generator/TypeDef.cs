using WinUIShell.Server;

namespace WinUIShell.Generator;

internal class TypeDef
{
    private readonly string _name = "";
    private readonly Api.TypeDef _apiTypeDef;
    private readonly TypeDef? _elementType;
    private readonly List<TypeDef>? _genericArguments;
    public bool IsNullable
    {
        get => _apiTypeDef.IsNullable;
    }
    public bool IsRpcSupportedType { get; internal set; }
    public bool IsInterface
    {
        get => _apiTypeDef.IsInterface;
    }
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

    public TypeDef(Api.TypeDef apiTypeDef)
    {
        _apiTypeDef = apiTypeDef;

        var serverTypeName = apiTypeDef.Name;
        IsRpcSupportedType = apiTypeDef.IsEnum;
        if (_apiTypeDef.IsInterface && serverTypeName.StartsWith("System."))
        {
            IsSystemInterface = true;
        }

        if (_apiTypeDef.IsGenericTypeParameter || _apiTypeDef.ElementType is not null)
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

        if (apiTypeDef.ElementType is not null)
        {
            _elementType = new TypeDef(apiTypeDef.ElementType);
        }
        if (apiTypeDef.GenericTypeArguments.Count > 0)
        {
            _genericArguments = [];
            foreach (var genericArgument in apiTypeDef.GenericTypeArguments)
            {
                _genericArguments.Add(new TypeDef(genericArgument));
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
        if (_apiTypeDef.IsArray)
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
        return _unsupportedTypes.Contains(_apiTypeDef.Name) || _apiTypeDef.IsDelegate;
    }

    private bool IsUnsupportedSystemInterface()
    {
        return IsSystemInterface && !_supportedSystemInterfaces.Contains(_apiTypeDef.Name);
    }

    private bool IsRefOrOut()
    {
        return _apiTypeDef.IsOut || (_apiTypeDef.IsByRef && !_apiTypeDef.IsIn);
    }

    private bool IsGenericParameter()
    {
        return _apiTypeDef.IsGenericTypeParameter || _apiTypeDef.IsGenericMethodParameter;
    }

    public string GetName()
    {
        if (_elementType is not null)
        {
            if (_apiTypeDef.IsArray)
            {
                return $"{_elementType.GetName()}[]";
            }
            return _elementType.GetName();
        }

        if (_genericArguments is not null)
        {
            return $"{_name}{GetGenericArgumentsExpression()}";
        }

        return _name;
    }

    public string GetGenericArgumentsExpression()
    {
        if (_genericArguments is not null)
        {
            var genericArgumentsNames = _genericArguments.Select(t => t.GetName());
            return $"<{string.Join(", ", genericArgumentsNames)}>";
        }
        return "";
    }

    public string GetTypeExpression()
    {
        string refExpression = "";
        if (_apiTypeDef.IsByRef)
        {
            if (_apiTypeDef.IsIn)
            {
                refExpression = "in ";
            }
            else
            if (_apiTypeDef.IsOut)
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
        if (_apiTypeDef.IsByRef)
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
