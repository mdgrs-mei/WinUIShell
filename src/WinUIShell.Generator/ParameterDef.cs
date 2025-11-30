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

    public ParameterDef(Api.ParameterDef apiParameterDef)
    {
        _apiParameterDef = apiParameterDef;
        Type = new TypeDef(apiParameterDef.Type);
    }

    public bool IsSupported()
    {
        return Type.IsSupported();
    }

    public bool IsUnsafe()
    {
        return Type.IsUnsafe();
    }

    public string GetSignatureExpression()
    {
        return $"{Type.GetTypeExpression()} {Name}";
    }

    public string GetArgumentExpression()
    {
        return Type.GetArgumentExpression(Name);
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
        foreach (var parameter in parameters)
        {
            _ = builder.Append($", {parameter.GetArgumentExpression()}");
        }
        return builder.ToString();
    }
}
