using RpcUIShell.Core;

namespace RpcUIShell.Generator;

internal class PropertyDef
{
    private readonly MemberDefType _memberDefType;
    private readonly List<ParameterDef>? _indexParameters;
    private readonly bool _hidesBase;
    private readonly bool _isOverride;
    private readonly bool _isVirtual;
    private readonly string _propertyName;

    public ObjectDef ObjectDef { get; }
    public TypeDef Type { get; }
    public TypeDef? ExplicitInterfaceType { get; }
    public bool CanRead { get; }
    public bool CanWrite { get; }
    public bool ImplementsInterface { get; }
    public bool IsIndexer
    {
        get => _indexParameters is not null;
    }
    public bool IsAbstract { get; }

    public PropertyDef(
        Api.PropertyDef apiPropertyDef,
        ObjectDef objectDef,
        MemberDefType memberDefType)
    {
        _hidesBase = apiPropertyDef.HidesBase;
        _isOverride = apiPropertyDef.IsOverride;

        // Additinally make abstract methods in classes virtual to provide default implementation because abstract classes need to be instantiated as return values.
        _isVirtual = apiPropertyDef.IsVirtual || (apiPropertyDef.IsAbstract && !objectDef.Type.IsInterface);
        // Remove abstract from methods in classes. Instead, make them virtual.
        IsAbstract = apiPropertyDef.IsAbstract && objectDef.Type.IsInterface;

        _propertyName = apiPropertyDef.Name;

        CanRead = apiPropertyDef.CanRead;
        CanWrite = apiPropertyDef.CanWrite;
        ImplementsInterface = apiPropertyDef.ImplementsInterface;

        ObjectDef = objectDef;
        _memberDefType = memberDefType;

        bool useSystemInterfaceName = apiPropertyDef.ImplementsGlobalSystemInterface;
        ExplicitInterfaceType = apiPropertyDef.ExplicitInterfaceType is null ?
            null :
            new TypeDef(apiPropertyDef.ExplicitInterfaceType, useSystemInterfaceName);

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
        string name,
        Api.MethodDef? getter,
        Api.MethodDef? setter,
        ObjectDef objectDef,
        MemberDefType memberDefType)
    {
        Api.MethodDef getterOrSetter = getter ?? setter!;

        _hidesBase = getterOrSetter.HidesBase;
        _isOverride = getterOrSetter.IsOverride;
        _isVirtual = getterOrSetter.IsVirtual || (getterOrSetter.IsAbstract && !objectDef.Type.IsInterface); ;
        IsAbstract = getterOrSetter.IsAbstract && objectDef.Type.IsInterface;
        _propertyName = name;

        CanRead = true;
        CanWrite = setter is not null;
        ImplementsInterface = getterOrSetter.ImplementsInterface;

        ObjectDef = objectDef;
        _memberDefType = memberDefType;

        bool useSystemInterfaceName = getterOrSetter.ImplementsGlobalSystemInterface;
        ExplicitInterfaceType = getterOrSetter.ExplicitInterfaceType is null ? null : new TypeDef(getterOrSetter.ExplicitInterfaceType, useSystemInterfaceName);

        List<Api.ParameterDef>? indexParameters = null;
        if (getter is not null)
        {
            indexParameters = getter.Parameters;
        }
        else if (setter!.Parameters is not null)
        {
            indexParameters = setter!.Parameters.GetRange(0, setter!.Parameters.Count - 1);
        }

        if (indexParameters is not null)
        {
            foreach (var apiParameterDef in indexParameters)
            {
                if (_indexParameters is null)
                {
                    _indexParameters = [];
                }
                _indexParameters.Add(new ParameterDef(apiParameterDef, useSystemInterfaceName));
            }
        }

        Api.TypeDef? typeDef = null;
        if (getter is not null)
        {
            typeDef = getter.ReturnType;
        }
        else if (setter!.Parameters is not null)
        {
            typeDef = setter!.Parameters.Last().Type;
        }

        Type = new TypeDef(typeDef!, useSystemInterfaceName);
    }

    public bool IsSupported()
    {
        if (!Type.IsSupported())
            return false;

        if (ExplicitInterfaceType is not null)
        {
            if (!ExplicitInterfaceType.IsSupported())
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

    public string GetName(bool isInterfaceImplExplicitImplementation = false, List<TypeDef>? genericTypeParametersOverride = null)
    {
        string interfaceTypeName = "";
        TypeDef? interfaceType = null;

        if (ExplicitInterfaceType is not null)
        {
            interfaceType = ExplicitInterfaceType;
        }
        else if (isInterfaceImplExplicitImplementation)
        {
            interfaceType = ObjectDef.Type;
        }

        if (interfaceType is not null)
        {
            if (genericTypeParametersOverride is not null)
            {
                interfaceType = interfaceType.OverrideGenericTypeParameter(genericTypeParametersOverride);
            }
            interfaceTypeName = $"{interfaceType.GetName()}.";
        }

        string name = IsIndexer ? "this" : _propertyName;
        return $"{interfaceTypeName}{name}";
    }

    public string GetOriginalName(bool isInterfaceImplExplicitImplementation = false, bool addInterfaceName = true)
    {
        string interfaceTypeName = "";
        if (addInterfaceName)
        {
            if (ExplicitInterfaceType is not null)
            {
                interfaceTypeName = $"{ExplicitInterfaceType.GetOriginalName()}.";
            }
            else if (isInterfaceImplExplicitImplementation)
            {
                interfaceTypeName = $"{ObjectDef.Type.GetOriginalName()}.";
            }
        }

        return $"{interfaceTypeName}{_propertyName}";
    }

    public string GetNameOfExpression(bool isInterfaceImplExplicitImplementation = false, bool addInterfaceName = true)
    {
        if (ExplicitInterfaceType is not null || isInterfaceImplExplicitImplementation)
        {
            return $"\"{GetOriginalName(isInterfaceImplExplicitImplementation, addInterfaceName)}\"";
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
        string accessorExpression = (ObjectDef.Type.IsInterface || ExplicitInterfaceType is not null) ? "" : "public ";
        string staticExpression = _memberDefType == MemberDefType.Static ? "static " : "";
        string newExpression = (_hidesBase && ExplicitInterfaceType is null) ? "new " : "";
        string overrideExpression = _isOverride ? "override " : "";
        string abstractExpression = IsAbstract ? "abstract " : "";
        string virtualExpression = (_isVirtual && !_isOverride && !IsAbstract && ExplicitInterfaceType is null) ? "virtual " : "";
        string indexerNameExpression = (IsIndexer && ExplicitInterfaceType is null) ? $"[global::System.Runtime.CompilerServices.IndexerName(\"{_propertyName}\")]\n" : "";
        string indexerParametersExpression = IsIndexer ? $"[{ParameterDef.GetParametersSignatureExpression(_indexParameters!, genericTypeParametersOverride: null, isExtensionMethod: false)}]" : "";

        return $"{indexerNameExpression}{unsafeExpression}{accessorExpression}{staticExpression}{newExpression}{overrideExpression}{abstractExpression}{virtualExpression}{Type.GetTypeExpression()} {GetName()}{indexerParametersExpression}";
    }

    public string GetInterfaceImplSignatureExpression(bool isExplicitImplementation, List<TypeDef>? genericTypeParametersOverride)
    {
        string unsafeExpression = Type.IsUnsafe() ? "unsafe " : "";
        string accessorExpression = (isExplicitImplementation || ExplicitInterfaceType is not null) ? "" : "public ";
        string staticExpression = _memberDefType == MemberDefType.Static ? "static " : "";
        string newExpression = "";
        string overrideExpression = "";
        string abstractExpression = "";
        string virtualExpression = "";
        string indexerParametersExpression = IsIndexer ? $"[{ParameterDef.GetParametersSignatureExpression(_indexParameters!, genericTypeParametersOverride, isExtensionMethod: false)}]" : "";

        TypeDef type = Type.OverrideGenericTypeParameter(genericTypeParametersOverride);
        return $"{unsafeExpression}{accessorExpression}{staticExpression}{newExpression}{overrideExpression}{abstractExpression}{virtualExpression}{type.GetTypeExpression()} {GetName(isExplicitImplementation, genericTypeParametersOverride)}{indexerParametersExpression}";
    }

    public string GetIndexerArgumentsExpression(List<TypeDef>? genericTypeParametersOverride)
    {
        if (!IsIndexer)
            return "";

        return ParameterDef.GetParametersArgumentExpression(_indexParameters!, genericTypeParametersOverride);
    }
}
