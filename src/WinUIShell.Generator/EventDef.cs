using WinUIShell.Server;

namespace WinUIShell.Generator;

internal class EventDef
{
    private readonly Api.EventDef _apiEventDef;
    private readonly MemberDefType _memberDefType;

    public ObjectDef ObjectDef { get; }
    public List<ParameterDef> Parameters { get; } = [];

    public EventDef(
        Api.EventDef apiEventDef,
        ObjectDef objectDef,
        MemberDefType memberDefType)
    {
        _apiEventDef = apiEventDef;
        ObjectDef = objectDef;
        _memberDefType = memberDefType;

        if (_apiEventDef.Parameters is not null)
        {
            foreach (var apiParameterDef in _apiEventDef.Parameters)
            {
                var parameter = new ParameterDef(apiParameterDef, useSystemInterfaceName: false);
                Parameters.Add(parameter);
            }
        }
    }

    public bool IsSupported()
    {
        // Only support (sender, eventArgs) pattern.
        if (Parameters.Count != 2)
            return false;

        foreach (var parameter in Parameters)
        {
            if (!parameter.IsSupported())
                return false;
        }
        return true;
    }

    public static string GetEventCallbackListExpression()
    {
        return $"private readonly EventCallbackList {GetEventCallbackListFieldName()} = new();";
    }

    private static string GetEventCallbackListFieldName()
    {
        return "_callbacks";
    }

    public string GetMethodFullName(bool isInterfaceImplExplicitImplementation = false)
    {
        string interfaceTypeName = isInterfaceImplExplicitImplementation ?
            $"{ObjectDef.Type.GetSystemInterfaceName()}." :
            "";

        return $"{interfaceTypeName}{GetMethodName()}";
    }

    private string GetMethodName()
    {
        return $"Add{GetEventName()}";
    }

    private string GetEventName()
    {
        return _apiEventDef.Name;
    }

    public string GetSignatureId()
    {
        return $"{GetMethodName()}({GetParametersSignatureId()})";
    }

    public string GetScriptBlockMethodExpression()
    {
        string accessorExpression = ObjectDef.Type.IsInterface ? "" : "public ";
        return GetScriptBlockMethodExpression(accessorExpression, isInterfaceImplExplicitImplementation: false);
    }

    public string GetInterfaceImplScriptBlockMethodExpression(bool isExplicitImplementation)
    {
        string accessorExpression = isExplicitImplementation ? "" : "public ";
        return GetScriptBlockMethodExpression(accessorExpression, isExplicitImplementation);
    }

    private string GetScriptBlockMethodExpression(string accessorExpression, bool isInterfaceImplExplicitImplementation)
    {
        string staticExpression = _memberDefType == MemberDefType.Static ? "static " : "";

        return $$"""
            {{accessorExpression}}{{staticExpression}}void {{GetMethodFullName(isInterfaceImplExplicitImplementation)}}(ScriptBlock scriptBlock, object? argumentList = null)
            {
                {{GetMethodName()}}(new EventCallback
                {
                    ScriptBlock = scriptBlock,
                    ArgumentList = argumentList
                });
            }
            """;
    }

    public string GetEventCallbackMethodExpression()
    {
        string accessorExpression = ObjectDef.Type.IsInterface ? "" : "public ";
        return GetEventCallbackMethodExpression(accessorExpression, isInterfaceImplExplicitImplementation: false);
    }

    public string GetInterfaceImplEventCallbackMethodExpression(bool isExplicitImplementation)
    {
        string accessorExpression = isExplicitImplementation ? "" : "public ";
        return GetEventCallbackMethodExpression(accessorExpression, isExplicitImplementation);
    }

    private string GetEventCallbackMethodExpression(string accessorExpression, bool isInterfaceImplExplicitImplementation)
    {
        string staticExpression = _memberDefType == MemberDefType.Static ? "static " : "";

        return $$"""
            {{accessorExpression}}{{staticExpression}}void {{GetMethodFullName(isInterfaceImplExplicitImplementation)}}(EventCallback eventCallback)
            {
                {{GetEventCallbackListFieldName()}}.Add(
                    WinUIShellObjectId,
                    "{{GetEventName()}}",
                    ObjectTypeMapping.Get().GetTargetTypeName<{{GetEventArgsTypeName()}}>(),
                    eventCallback);
            }
            """;
    }

    private string GetEventArgsTypeName()
    {
        return Parameters[1].Type.GetName();
    }

    private string GetParametersSignatureId()
    {
        return ParameterDef.GetParametersSignatureId(Parameters);
    }
}
