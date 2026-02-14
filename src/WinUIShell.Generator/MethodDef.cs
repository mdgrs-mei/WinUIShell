using WinUIShell.ApiExporter;

namespace WinUIShell.Generator;

internal class MethodDef
{
    private readonly Api.MethodDef _apiMethodDef;
    private readonly MemberDefType _memberDefType;
    private readonly bool _isUnsafe;

    public ObjectDef ObjectDef { get; }
    public TypeDef? ReturnType { get; }
    public List<ParameterDef> Parameters { get; } = [];
    public TypeDef? ExplicitInterfaceType;
    private bool IsVirtual
    {
        // Additinally make abstract methods in classes virtual to provide default implementation because abstract classes need to be instantiated as return values.
        get => _apiMethodDef.IsVirtual || (_apiMethodDef.IsAbstract && !ObjectDef.Type.IsInterface);
    }
    private bool IsAbstract
    {
        // Remove abstract from methods in classes. Instead, make them virtual.
        get => _apiMethodDef.IsAbstract && ObjectDef.Type.IsInterface;
    }
    public bool ImplementsInterface
    {
        get => _apiMethodDef.ImplementsInterface;
    }
    public bool ImplementsSystemInterface
    {
        get => _apiMethodDef.ImplementsSystemInterface;
    }

    public MethodDef(
        Api.MethodDef apiMethodDef,
        ObjectDef objectDef,
        MemberDefType memberDefType)
    {
        _apiMethodDef = apiMethodDef;
        ObjectDef = objectDef;
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
            if (Api.IsUnsupportedMethod(_apiMethodDef.Name!))
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

    public string GetName(bool isInterfaceImplExplicitImplementation = false)
    {
        string interfaceTypeName = "";
        if (ExplicitInterfaceType is not null)
        {
            interfaceTypeName = $"{ExplicitInterfaceType.GetName()}.";
        }
        else
        if (isInterfaceImplExplicitImplementation)
        {
            interfaceTypeName = $"{ObjectDef.Type.GetSystemInterfaceName()}.";
        }

        return $"{interfaceTypeName}{_apiMethodDef.Name}";
    }

    private string GetOriginalName(bool isInterfaceImplExplicitImplementation = false)
    {
        string interfaceTypeName = "";
        if (ExplicitInterfaceType is not null)
        {
            interfaceTypeName = $"{ExplicitInterfaceType.GetOriginalName()}.";
        }
        else
        if (isInterfaceImplExplicitImplementation)
        {
            interfaceTypeName = $"{ObjectDef.Type.GetOriginalName()}.";
        }

        return $"{interfaceTypeName}{_apiMethodDef.Name}";
    }

    public string GetNameOfExpression(bool isInterfaceImplExplicitImplementation = false)
    {
        if (ExplicitInterfaceType is not null || isInterfaceImplExplicitImplementation)
        {
            return $"\"{GetOriginalName(isInterfaceImplExplicitImplementation)}\"";
        }
        else
        {
            return $"nameof({GetName()})";
        }
    }

    public string GetSignatureId()
    {
        return $"{GetName()}({GetParametersSignatureId()})";
    }

    public string GetSignatureExpression()
    {
        string unsafeExpression = _isUnsafe ? "unsafe " : "";
        string accessorExpression = (ObjectDef.Type.IsInterface || ExplicitInterfaceType is not null) ? "" : "public ";
        string staticExpression = _memberDefType == MemberDefType.Static ? "static " : "";
        string newExpression = _apiMethodDef.HidesBase ? "new " : "";
        string overrideExpression = _apiMethodDef.IsOverride ? "override " : "";
        string abstractExpression = IsAbstract ? "abstract " : "";
        string virtualExpression = (IsVirtual && !_apiMethodDef.IsOverride && !IsAbstract && ExplicitInterfaceType is null) ? "virtual " : "";

        return $"{unsafeExpression}{accessorExpression}{staticExpression}{newExpression}{overrideExpression}{abstractExpression}{virtualExpression}{ReturnType!.GetTypeExpression()} {GetName()}({GetParametersExpression(genericTypeParametersOverride: null)})";
    }

    public string GetInterfaceImplSignatureExpression(bool isExplicitImplementation, List<TypeDef>? genericTypeParametersOverride)
    {
        string unsafeExpression = _isUnsafe ? "unsafe " : "";
        string accessorExpression = isExplicitImplementation ? "" : "public ";
        string staticExpression = _memberDefType == MemberDefType.Static ? "static " : "";
        string newExpression = "";
        string overrideExpression = _apiMethodDef.Name == "ToString" ? "override " : "";
        string abstractExpression = "";
        string virtualExpression = "";

        TypeDef returnType = ReturnType!.OverrideGenericTypeParameter(genericTypeParametersOverride);

        return $"{unsafeExpression}{accessorExpression}{staticExpression}{newExpression}{overrideExpression}{abstractExpression}{virtualExpression}{returnType.GetTypeExpression()} {GetName(isExplicitImplementation)}({GetParametersExpression(genericTypeParametersOverride)})";
    }

    public string GetConstructorSignatureExpression(string className)
    {
        string unsafeExpression = _isUnsafe ? "unsafe " : "";
        string accessorExpression = ObjectDef.Type.IsInterface ? "" : "public ";
        return $"{unsafeExpression}{accessorExpression}{className}({GetParametersExpression(genericTypeParametersOverride: null)})";
    }

    private string GetParametersSignatureId()
    {
        return ParameterDef.GetParametersSignatureId(Parameters);
    }

    private string GetParametersExpression(List<TypeDef>? genericTypeParametersOverride)
    {
        return ParameterDef.GetParametersSignatureExpression(Parameters, genericTypeParametersOverride);
    }

    public string GetArgumentsExpression()
    {
        return ParameterDef.GetParametersArgumentExpression(Parameters);
    }
}
