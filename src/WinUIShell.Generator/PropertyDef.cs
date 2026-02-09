using WinUIShell.Server;

namespace WinUIShell.Generator;

internal class PropertyDef
{
    private readonly ObjectDef _objectDef;
    private readonly MemberDefType _memberDefType;
    private readonly TypeDef? _explicitInterfaceType;
    private readonly List<ParameterDef>? _indexParameters;
    private readonly bool _hidesBase;
    private readonly bool _isOverride;
    private readonly bool _isVirtual;
    private readonly string _propertyName;

    public readonly TypeDef Type;
    public bool CanRead { get; private set; }
    public bool CanWrite { get; private set; }
    public bool IsAbstract { get; private set; }
    public bool ImplementsInterface { get; private set; }
    public bool IsIndexer
    {
        get => _indexParameters is not null;
    }

    public PropertyDef(
        Api.PropertyDef apiPropertyDef,
        ObjectDef objectDef,
        MemberDefType memberDefType)
    {
        _hidesBase = apiPropertyDef.HidesBase;
        _isOverride = apiPropertyDef.IsOverride;
        _isVirtual = apiPropertyDef.IsVirtual;
        _propertyName = apiPropertyDef.Name;

        CanRead = apiPropertyDef.CanRead;
        CanWrite = apiPropertyDef.CanWrite;
        IsAbstract = apiPropertyDef.IsAbstract;
        ImplementsInterface = apiPropertyDef.ImplementsInterface;

        _objectDef = objectDef;
        _memberDefType = memberDefType;

        bool useSystemInterfaceName = apiPropertyDef.ImplementsSystemInterface;
        _explicitInterfaceType = apiPropertyDef.ExplicitInterfaceType is null ? null : new TypeDef(apiPropertyDef.ExplicitInterfaceType, useSystemInterfaceName);

        if (apiPropertyDef.IndexParameters is not null)
        {
            foreach (var apiParameterDef in apiPropertyDef.IndexParameters)
            {
                if (_indexParameters is null)
                {
                    _indexParameters = [];
                }
                _indexParameters.Add(new ParameterDef(apiParameterDef, useSystemInterfaceName));
            }
        }

        Type = new TypeDef(apiPropertyDef.Type, useSystemInterfaceName);
    }

    public PropertyDef(
        Api.MethodDef indexerGetter,
        Api.MethodDef? indexerSetter,
        ObjectDef objectDef,
        MemberDefType memberDefType)
    {
        _hidesBase = indexerGetter.HidesBase;
        _isOverride = indexerGetter.IsOverride;
        _isVirtual = indexerGetter.IsVirtual;
        _propertyName = "Item";

        CanRead = true;
        CanWrite = indexerSetter is not null;
        IsAbstract = indexerGetter.IsAbstract;
        ImplementsInterface = indexerGetter.ImplementsInterface;

        _objectDef = objectDef;
        _memberDefType = memberDefType;

        bool useSystemInterfaceName = indexerGetter!.ImplementsSystemInterface;
        _explicitInterfaceType = indexerGetter.ExplicitInterfaceType is null ? null : new TypeDef(indexerGetter.ExplicitInterfaceType, useSystemInterfaceName);

        List<Api.ParameterDef> indexParameters = indexerGetter.Parameters!;

        foreach (var apiParameterDef in indexParameters)
        {
            if (_indexParameters is null)
            {
                _indexParameters = [];
            }
            _indexParameters.Add(new ParameterDef(apiParameterDef, useSystemInterfaceName));
        }

        Type = new TypeDef(indexerGetter.ReturnType!, useSystemInterfaceName);
    }

    public bool IsSupported()
    {
        if (!Type.IsSupported())
            return false;

        if (_explicitInterfaceType is not null)
        {
            if (!_explicitInterfaceType.IsSupported())
                return false;
        }

        if (_indexParameters is not null)
        {
            foreach (var parameter in _indexParameters)
            {
                if (!parameter.IsSupported())
                    return false;
            }
        }

        return true;
    }

    public string GetName(bool isInterfaceImplExplicitImplementation = false)
    {
        string interfaceTypeName = "";
        if (_explicitInterfaceType is not null)
        {
            interfaceTypeName = $"{_explicitInterfaceType.GetName()}.";
        }
        else
        if (isInterfaceImplExplicitImplementation)
        {
            interfaceTypeName = $"{_objectDef.Type.GetName()}.";
        }

        string name = IsIndexer ? "this" : _propertyName;
        return $"{interfaceTypeName}{name}";
    }

    public string GetOriginalName(bool isInterfaceImplExplicitImplementation = false)
    {
        string interfaceTypeName = "";
        if (_explicitInterfaceType is not null)
        {
            interfaceTypeName = $"{_explicitInterfaceType.GetOriginalName()}.";
        }
        else
        if (isInterfaceImplExplicitImplementation)
        {
            interfaceTypeName = $"{_objectDef.Type.GetOriginalName()}.";
        }

        return $"{interfaceTypeName}{_propertyName}";
    }

    public string GetNameOfExpression(bool isInterfaceImplExplicitImplementation = false)
    {
        if (_explicitInterfaceType is not null || isInterfaceImplExplicitImplementation)
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
        if (IsIndexer)
        {
            return $"{GetName()}[{ParameterDef.GetParametersSignatureId(_indexParameters!)}]";
        }
        else
        {
            return GetName();
        }
    }

    public string GetSignatureExpression()
    {
        string unsafeExpression = Type.IsUnsafe() ? "unsafe " : "";
        string accessorExpression = (_objectDef.Type.IsInterface || _explicitInterfaceType is not null) ? "" : "public ";
        string staticExpression = _memberDefType == MemberDefType.Static ? "static " : "";
        string newExpression = _hidesBase ? "new " : "";
        string overrideExpression = _isOverride ? "override " : "";
        string abstractExpression = IsAbstract ? "abstract " : "";
        string virtualExpression = (_isVirtual && !_isOverride && !IsAbstract && _explicitInterfaceType is null) ? "virtual " : "";
        string indexerNameExpression = (IsIndexer && _explicitInterfaceType is null) ? $"[global::System.Runtime.CompilerServices.IndexerName(\"{_propertyName}\")]\n" : "";
        string indexerParametersExpression = IsIndexer ? $"[{ParameterDef.GetParametersSignatureExpression(_indexParameters!, genericTypeParametersOverride: null)}]" : "";

        return $"{indexerNameExpression}{unsafeExpression}{accessorExpression}{staticExpression}{newExpression}{overrideExpression}{abstractExpression}{virtualExpression}{Type.GetTypeExpression()} {GetName()}{indexerParametersExpression}";
    }

    public string GetInterfaceImplSignatureExpression(bool isExplicitImplementation, List<TypeDef>? genericTypeParametersOverride)
    {
        string unsafeExpression = Type.IsUnsafe() ? "unsafe " : "";
        string accessorExpression = isExplicitImplementation ? "" : "public ";
        string staticExpression = _memberDefType == MemberDefType.Static ? "static " : "";
        string newExpression = "";
        string overrideExpression = "";
        string abstractExpression = "";
        string virtualExpression = "";
        string indexerParametersExpression = IsIndexer ? $"[{ParameterDef.GetParametersSignatureExpression(_indexParameters!, genericTypeParametersOverride)}]" : "";

        TypeDef type = Type.OverrideGenericTypeParameter(genericTypeParametersOverride);
        return $"{unsafeExpression}{accessorExpression}{staticExpression}{newExpression}{overrideExpression}{abstractExpression}{virtualExpression}{type.GetTypeExpression()} {GetName(isExplicitImplementation)}{indexerParametersExpression}";
    }

    public string GetIndexerArgumentsExpression()
    {
        if (!IsIndexer)
            return "";

        return ParameterDef.GetParametersArgumentExpression(_indexParameters!);
    }
}
