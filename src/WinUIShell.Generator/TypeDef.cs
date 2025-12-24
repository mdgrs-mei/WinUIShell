using WinUIShell.Server;

namespace WinUIShell.Generator;

internal class TypeDef
{
    private readonly string _name = "";
    private readonly Api.TypeDef _apiTypeDef;
    private readonly TypeDef? _elementType;

    public bool AlwaysReturnSystemInterfaceName { get; set; }
    public readonly List<TypeDef>? GenericArguments;
    public bool IsNullable
    {
        get => _apiTypeDef.IsNullable;
    }
    public bool IsRpcSupportedType { get; private set; }
    public bool IsInterface
    {
        get => _apiTypeDef.IsInterface;
    }
    public bool IsSystemInterface { get; private set; }
    public bool IsObject { get; private set; }
    public bool IsVoid { get; private set; }
    public bool IsArray
    {
        get => _apiTypeDef.IsArray;
    }

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
        "WinRT.IObjectReference",
        "WinRT.ObjectReference",
    ];

    public TypeDef(Api.TypeDef apiTypeDef, bool alwaysReturnSystemInterfaceName = false)
    {
        _apiTypeDef = apiTypeDef;
        AlwaysReturnSystemInterfaceName = alwaysReturnSystemInterfaceName;

        var serverTypeName = apiTypeDef.Name;
        IsRpcSupportedType = apiTypeDef.IsEnum;
        if (_apiTypeDef.IsInterface && _apiTypeDef.IsSystemObject)
        {
            IsSystemInterface = true;
            _name = $"WinUIShell.{serverTypeName}";
        }
        else
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
            _elementType = new TypeDef(apiTypeDef.ElementType, AlwaysReturnSystemInterfaceName);
        }
        if (apiTypeDef.GenericTypeArguments is not null)
        {
            foreach (var genericArgument in apiTypeDef.GenericTypeArguments)
            {
                if (GenericArguments is null)
                {
                    GenericArguments = [];
                }
                GenericArguments.Add(new TypeDef(genericArgument, AlwaysReturnSystemInterfaceName));
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
        if (IsArray)
            return false;

        if (IsRefOrOut())
            return false;

        if (_apiTypeDef.IsPointer)
            return false;

        if (_apiTypeDef.IsFunctionPointer)
            return false;

        if (IsGenericParameter())
            return true;

        if (IsUnsupportedType())
            return false;

        if (_elementType is not null && !_elementType.IsSupported())
            return false;

        return true;
    }

    private bool IsUnsupportedType()
    {
        bool isUnsupportedSystemInterface = IsSystemInterface && !Api.IsSupportedSystemInterface(_apiTypeDef.Name);
        return _unsupportedTypes.Contains(_apiTypeDef.Name) || _apiTypeDef.IsDelegate || isUnsupportedSystemInterface;
    }

    private bool IsRefOrOut()
    {
        return _apiTypeDef.IsOut || (_apiTypeDef.IsByRef && !_apiTypeDef.IsIn);
    }

    private bool IsGenericParameter()
    {
        return _apiTypeDef.IsGenericTypeParameter || _apiTypeDef.IsGenericMethodParameter;
    }

    public bool IsUnsafe()
    {
        if (_apiTypeDef.IsPointer)
            return true;

        if (_elementType is not null && _elementType.IsUnsafe())
            return true;

        return false;
    }

    public string GetName()
    {
        if (AlwaysReturnSystemInterfaceName)
        {
            return GetSystemInterfaceName();
        }
        else
        {
            return GetName(_name);
        }
    }

    public string GetSystemInterfaceName()
    {
        if (IsSystemInterface)
        {
            return GetName($"global::{_apiTypeDef.Name}");
        }
        else
        {
            return GetName(_name);
        }
    }

    private string GetName(string thisName)
    {
        if (_elementType is not null)
        {
            if (IsArray)
            {
                return $"{_elementType.GetName()}[]";
            }
            else
            if (_apiTypeDef.IsPointer)
            {
                return $"{_elementType.GetName()}*";
            }
            return _elementType.GetName();
        }

        var name = _apiTypeDef.IsPointer ? $"{thisName}*" : thisName;
        if (GenericArguments is not null)
        {
            return $"{name}{GetGenericArgumentsExpression()}";
        }
        else
        {
            return name;
        }
    }

    public string GetId()
    {
        if (_elementType is not null)
        {
            if (IsArray)
            {
                return $"{_elementType.GetId()}[]";
            }
            else
            if (_apiTypeDef.IsPointer)
            {
                return $"{_elementType.GetId()}*";
            }
            return _elementType.GetId();
        }

        var name = _apiTypeDef.Name;
        if (GenericArguments is not null)
        {
            return $"{name}`{GenericArguments.Count}";
        }
        else
        {
            return name;
        }
    }

    public string GetGenericArgumentsExpression(List<TypeDef>? parentGenericArguments = null)
    {
        if (GenericArguments is not null)
        {
            var genericArgumentsNames = GenericArguments.Select(t => t.GetName());

            // In a nested class, generic arguments defined in the parent class can be omitted.
            if (parentGenericArguments is not null)
            {
                var parentGenericArgumentsNames = parentGenericArguments.Select(t => t.GetName());
                genericArgumentsNames = genericArgumentsNames.Where(t => !parentGenericArgumentsNames.Contains(t));
            }

            if (genericArgumentsNames.Count() != 0)
            {
                return $"<{string.Join(", ", genericArgumentsNames)}>";
            }
        }
        return "";
    }

    public string GetTypeExpression()
    {
        string refExpression = GetRefExpression();
        return $"{refExpression}{GetName()}{(IsNullable ? "?" : "")}";
    }

    private string GetRefExpression()
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
        return refExpression;
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
            return "(value is IWinUIShellObject v ? v.WinUIShellObjectId : value)";
        }
        else
        {
            return "value?.WinUIShellObjectId";
        }
    }

    public string GetArgumentExpression(string variableName, int variableIndex)
    {
        if (_apiTypeDef.IsByRef)
        {
            return _elementType!.GetArgumentExpression(variableName, variableIndex);
        }
        else
        if (IsRpcSupportedType)
        {
            return variableName;
        }
        else
        if (IsObject || IsGenericParameter())
        {
            return $"({variableName} is IWinUIShellObject v{variableIndex} ? v{variableIndex}.WinUIShellObjectId : {variableName})";
        }
        else
        {
            return $"{variableName}?.WinUIShellObjectId";
        }
    }
}
