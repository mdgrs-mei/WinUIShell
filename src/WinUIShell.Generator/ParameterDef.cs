using System.Text;
using WinUIShell.Server;

namespace WinUIShell.Generator;

internal class ParameterDef
{
    private readonly Api.ParameterDef _apiParameterDef;

    public TypeDef Type;
    public string Name
    {
        get => _apiParameterDef.Name!;
    }

    public ParameterDef(Api.ParameterDef apiParameterDef, bool useSystemInterfaceName)
    {
        _apiParameterDef = apiParameterDef;
        Type = new TypeDef(apiParameterDef.Type, useSystemInterfaceName);
    }

    public bool IsSupported()
    {
        return Type.IsSupported();
    }

    public bool IsUnsafe()
    {
        return Type.IsUnsafe();
    }

    public string GetSignatureId()
    {
        return $"{Type.GetId()}";
    }

    public string GetSignatureExpression()
    {
        return $"{Type.GetTypeExpression()} {Name}";
    }

    public string GetArgumentExpression(int parameterIndex)
    {
        return Type.GetArgumentExpression(Name, parameterIndex);
    }

    public static string GetParametersSignatureId(List<ParameterDef> parameters)
    {
        if (parameters.Count == 0)
            return "";

        StringBuilder builder = new();
        string commaSpace = "";
        foreach (var parameter in parameters)
        {
            _ = builder.Append($"{commaSpace}{parameter.GetSignatureId()}");
            commaSpace = ", ";
        }
        return builder.ToString();
    }

    public static string GetParametersSignatureExpression(List<ParameterDef> parameters)
    {
        if (parameters.Count == 0)
            return "";

        StringBuilder builder = new();
        string commaSpace = "";
        foreach (var parameter in parameters)
        {
            _ = builder.Append($"{commaSpace}{parameter.GetSignatureExpression()}");
            commaSpace = ", ";
        }
        return builder.ToString();
    }

    public static string GetParametersArgumentExpression(List<ParameterDef> parameters)
    {
        if (parameters.Count == 0)
            return "";

        StringBuilder builder = new();
        int parameterIndex = 0;
        foreach (var parameter in parameters)
        {
            _ = builder.Append($", {parameter.GetArgumentExpression(parameterIndex)}");
            ++parameterIndex;
        }
        return builder.ToString();
    }
}
