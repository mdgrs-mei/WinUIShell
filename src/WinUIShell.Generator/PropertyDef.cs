using WinUIShell.Server;

namespace WinUIShell.Generator;

internal class PropertyDef
{
    private readonly Api.PropertyDef _apiPropertyDef;
    private readonly ObjectDef _objectDef;
    private readonly MemberDefType _memberDefType;
    public readonly TypeDef Type;
    private readonly TypeDef? _explicitInterfaceType;
    public bool CanRead
    {
        get => _apiPropertyDef.CanRead;
    }
    public bool CanWrite
    {
        get => _apiPropertyDef.CanWrite;
    }
    public bool IsAbstract
    {
        get => _apiPropertyDef.IsAbstract;
    }

    public PropertyDef(
        Api.PropertyDef apiPropertyDef,
        ObjectDef objectDef,
        MemberDefType memberDefType)
    {
        _apiPropertyDef = apiPropertyDef;
        _objectDef = objectDef;
        _memberDefType = memberDefType;
        Type = new TypeDef(_apiPropertyDef.Type);
        _explicitInterfaceType = apiPropertyDef.ExplicitInterfaceType is null ? null : new TypeDef(apiPropertyDef.ExplicitInterfaceType);
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

        return true;
    }

    public string GetName()
    {
        string interfaceTypeName = _explicitInterfaceType is null ? "" : $"{_explicitInterfaceType.GetName()}.";
        return $"{interfaceTypeName}{_apiPropertyDef.Name}";
    }

    public string GetSignatureExpression()
    {
        string accessorExpression = (_objectDef.Type.IsInterface || _explicitInterfaceType is not null) ? "" : "public ";
        string staticExpression = _memberDefType == MemberDefType.Static ? "static " : "";
        string newExpression = _apiPropertyDef.HidesBase ? "new " : "";
        string overrideExpression = _apiPropertyDef.IsOverride ? "override " : "";
        string abstractExpression = _apiPropertyDef.IsAbstract ? "abstract " : "";
        string virtualExpression = (_apiPropertyDef.IsVirtual && !_apiPropertyDef.IsOverride && !_apiPropertyDef.IsAbstract && _explicitInterfaceType is null) ? "virtual " : "";

        return $"{accessorExpression}{staticExpression}{newExpression}{overrideExpression}{abstractExpression}{virtualExpression}{Type.GetTypeExpression()} {GetName()}";
    }
}
