using WinUIShell.Server;

namespace WinUIShell.Generator;

internal class PropertyDef
{
    private readonly Api.PropertyDef _apiPropertyDef;
    private readonly ObjectDef _objectDef;
    private readonly MemberDefType _memberDefType;
    public readonly TypeDef Type;
    public string Name
    {
        get => _apiPropertyDef.Name;
    }
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
    }

    public bool IsSupported()
    {
        return Type.IsSupported();
    }

    public string GetSignatureExpression()
    {
        string accessorExpression = _objectDef.Type.IsInterface ? "" : "public ";
        string staticExpression = _memberDefType == MemberDefType.Static ? "static " : "";
        string newExpression = _apiPropertyDef.HidesBase ? "new " : "";
        string overrideExpression = _apiPropertyDef.IsOverride ? "override " : "";
        string abstructExpression = _apiPropertyDef.IsAbstract ? "abstract " : "";
        return $"{accessorExpression}{staticExpression}{newExpression}{overrideExpression}{abstructExpression}{Type.GetTypeExpression()} {Name}";
    }
}
