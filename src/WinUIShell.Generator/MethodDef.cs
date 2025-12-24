using WinUIShell.Server;

namespace WinUIShell.Generator;

internal class MethodDef
{
    private readonly Api.MethodDef _apiMethodDef;
    private readonly ObjectDef _objectDef;
    private readonly MemberDefType _memberDefType;
    private readonly bool _isUnsafe;

    public TypeDef? ReturnType { get; }
    public List<ParameterDef> Parameters { get; } = [];
    public TypeDef? ExplicitInterfaceType;
    public bool IsAbstract
    {
        get => _apiMethodDef.IsAbstract;
    }

    private static readonly List<string> _unsupportedMethodNames =
    [
        "Equals",
        "GetHashCode",
        "GetType",
    ];

    public MethodDef(
        Api.MethodDef apiMethodDef,
        ObjectDef objectDef,
        MemberDefType memberDefType)
    {
        _apiMethodDef = apiMethodDef;
        _objectDef = objectDef;
        _memberDefType = memberDefType;

        bool useSystemInterfaceName = _apiMethodDef.ImplementsSystemInterface;

        if (apiMethodDef.ReturnType is not null)
        {
            ReturnType = new TypeDef(apiMethodDef.ReturnType, useSystemInterfaceName);
            if (ReturnType.IsUnsafe())
            {
                _isUnsafe = true;
            }
        }

        if (_apiMethodDef.Parameters is not null)
        {
            foreach (var apiParameterDef in _apiMethodDef.Parameters)
            {
                var parameter = new ParameterDef(apiParameterDef, useSystemInterfaceName);
                Parameters.Add(parameter);
                if (parameter.IsUnsafe())
                {
                    _isUnsafe = true;
                }
            }
        }

        ExplicitInterfaceType = apiMethodDef.ExplicitInterfaceType is null ? null : new TypeDef(apiMethodDef.ExplicitInterfaceType, useSystemInterfaceName);
    }

    public bool IsSupported()
    {
        if (_apiMethodDef.IsGenericMethod)
            return false;

        if (!string.IsNullOrEmpty(_apiMethodDef.Name))
        {
            if (_unsupportedMethodNames.Contains(_apiMethodDef.Name!))
                return false;
        }

        if (ReturnType is not null)
        {
            if (!ReturnType.IsSupported())
                return false;
        }

        foreach (var parameter in Parameters)
        {
            if (!parameter.IsSupported())
                return false;
        }

        if (ExplicitInterfaceType is not null)
        {
            if (!ExplicitInterfaceType.IsSupported())
                return false;
        }

        return true;
    }

    public string GetName(bool isExplicitImplementation = false)
    {
        string interfaceTypeName = "";
        if (ExplicitInterfaceType is not null)
        {
            interfaceTypeName = $"{ExplicitInterfaceType.GetName()}.";
        }
        else
        if (isExplicitImplementation)
        {
            interfaceTypeName = $"{_objectDef.Type.GetSystemInterfaceName()}.";
        }

        return $"{interfaceTypeName}{_apiMethodDef.Name}";
    }

    public string GetSignatureId()
    {
        return $"{GetName()}({GetParametersSignatureId()})";
    }

    public string GetSignatureExpression()
    {
        string unsafeExpression = _isUnsafe ? "unsafe " : "";
        string accessorExpression = (_objectDef.Type.IsInterface || ExplicitInterfaceType is not null) ? "" : "public ";
        string staticExpression = _memberDefType == MemberDefType.Static ? "static " : "";
        string newExpression = _apiMethodDef.HidesBase ? "new " : "";
        string overrideExpression = _apiMethodDef.IsOverride ? "override " : "";
        string abstractExpression = IsAbstract ? "abstract " : "";
        string virtualExpression = (_apiMethodDef.IsVirtual && !_apiMethodDef.IsOverride && !_apiMethodDef.IsAbstract && ExplicitInterfaceType is null) ? "virtual " : "";

        return $"{unsafeExpression}{accessorExpression}{staticExpression}{newExpression}{overrideExpression}{abstractExpression}{virtualExpression}{ReturnType!.GetTypeExpression()} {GetName()}({GetParametersExpression()})";
    }

    public string GetInterfaceImplSignatureExpression(bool isExplicitImplementation)
    {
        string unsafeExpression = _isUnsafe ? "unsafe " : "";
        string accessorExpression = isExplicitImplementation ? "" : "public ";
        string staticExpression = _memberDefType == MemberDefType.Static ? "static " : "";
        string newExpression = "";
        string overrideExpression = "";
        string abstractExpression = "";
        string virtualExpression = "";

        return $"{unsafeExpression}{accessorExpression}{staticExpression}{newExpression}{overrideExpression}{abstractExpression}{virtualExpression}{ReturnType!.GetTypeExpression()} {GetName(isExplicitImplementation)}({GetParametersExpression()})";
    }

    public string GetConstructorSignatureExpression(string className)
    {
        string unsafeExpression = _isUnsafe ? "unsafe " : "";
        string accessorExpression = _objectDef.Type.IsInterface ? "" : "public ";
        return $"{unsafeExpression}{accessorExpression}{className}({GetParametersExpression()})";
    }

    private string GetParametersSignatureId()
    {
        return ParameterDef.GetParametersSignatureId(Parameters);
    }

    private string GetParametersExpression()
    {
        return ParameterDef.GetParametersSignatureExpression(Parameters);
    }

    public string GetArgumentsExpression()
    {
        return ParameterDef.GetParametersArgumentExpression(Parameters);
    }
}
