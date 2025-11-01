using WinUIShell.Server;

namespace WinUIShell.Generator;

internal class PropertyDef
{
    private readonly Api.PropertyDef _apiPropertyDef;
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

    public PropertyDef(Api.PropertyDef apiPropertyDef)
    {
        _apiPropertyDef = apiPropertyDef;
        Type = new TypeDef(_apiPropertyDef.Type);
    }

    public bool IsSupported()
    {
        return Type.IsSupported();
    }

    public string GetSignatureExpression()
    {
        return $"{Type.GetTypeExpression()} {Name}";
    }
}
