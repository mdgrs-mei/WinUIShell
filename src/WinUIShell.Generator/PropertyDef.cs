using WinUIShell.Server;

namespace WinUIShell.Generator;

internal class PropertyDef
{
    private readonly Api.PropertyDef _apiPropertyDef;
    private readonly TypeDef _type;

    public PropertyDef(Api.PropertyDef apiPropertyDef)
    {
        _apiPropertyDef = apiPropertyDef;
        _type = new TypeDef(_apiPropertyDef.Type);
    }
}
