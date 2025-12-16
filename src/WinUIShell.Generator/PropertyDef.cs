using WinUIShell.Server;

namespace WinUIShell.Generator;

internal class PropertyDef
{
    private readonly Api.PropertyDef _apiPropertyDef;
    private readonly ObjectDef _objectDef;
    private readonly MemberDefType _memberDefType;
    private readonly TypeDef? _explicitInterfaceType;
    private readonly List<ParameterDef>? _indexParameters;

    public readonly TypeDef Type;
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
    public bool IsIndexer
    {
        get => _indexParameters is not null;
    }

    public PropertyDef(
        Api.PropertyDef apiPropertyDef,
        ObjectDef objectDef,
        MemberDefType memberDefType)
    {
        _apiPropertyDef = apiPropertyDef;
        _objectDef = objectDef;
        _memberDefType = memberDefType;

        bool useSystemInterfaceName = _apiPropertyDef.ImplementsSystemInterface;
        _explicitInterfaceType = apiPropertyDef.ExplicitInterfaceType is null ? null : new TypeDef(apiPropertyDef.ExplicitInterfaceType, useSystemInterfaceName);

        if (_apiPropertyDef.IndexParameters is not null)
        {
            foreach (var apiParameterDef in _apiPropertyDef.IndexParameters)
            {
                if (_indexParameters is null)
                {
                    _indexParameters = [];
                }
                _indexParameters.Add(new ParameterDef(apiParameterDef, useSystemInterfaceName));
            }
        }

        Type = new TypeDef(_apiPropertyDef.Type, useSystemInterfaceName);
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

    public string GetName()
    {
        string interfaceTypeName = _explicitInterfaceType is null ? "" : $"{_explicitInterfaceType.GetName()}.";
        string name = IsIndexer ? "this" : _apiPropertyDef.Name;
        return $"{interfaceTypeName}{name}";
    }

    public string GetSignatureExpression()
    {
        string unsafeExpression = Type.IsUnsafe() ? "unsafe " : "";
        string accessorExpression = (_objectDef.Type.IsInterface || _explicitInterfaceType is not null) ? "" : "public ";
        string staticExpression = _memberDefType == MemberDefType.Static ? "static " : "";
        string newExpression = _apiPropertyDef.HidesBase ? "new " : "";
        string overrideExpression = _apiPropertyDef.IsOverride ? "override " : "";
        string abstractExpression = _apiPropertyDef.IsAbstract ? "abstract " : "";
        string virtualExpression = (_apiPropertyDef.IsVirtual && !_apiPropertyDef.IsOverride && !_apiPropertyDef.IsAbstract && _explicitInterfaceType is null) ? "virtual " : "";
        string indexerParametersExpression = IsIndexer ? $"[{ParameterDef.GetParametersSignatureExpression(_indexParameters!)}]" : "";

        return $"{unsafeExpression}{accessorExpression}{staticExpression}{newExpression}{overrideExpression}{abstractExpression}{virtualExpression}{Type.GetTypeExpression()} {GetName()}{indexerParametersExpression}";
    }

    public string GetIndexerArgumentsExpression()
    {
        if (!IsIndexer)
            return "";

        return ParameterDef.GetParametersArgumentExpression(_indexParameters!);
    }
}
