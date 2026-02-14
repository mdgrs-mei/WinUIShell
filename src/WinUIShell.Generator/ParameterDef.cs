using System.Text;
using WinUIShell.ApiExporter;

namespace WinUIShell.Generator;

internal class ParameterDef
{
    private readonly Api.ParameterDef _apiParameterDef;

    public TypeDef Type;
    public string Name { get; set; }

    public ParameterDef(Api.ParameterDef apiParameterDef, bool useSystemInterfaceName)
    {
        _apiParameterDef = apiParameterDef;
        Type = new TypeDef(apiParameterDef.Type, useSystemInterfaceName);

        Name = _apiParameterDef.Name!;

        // event is a keyword, object is a type name.
        if (Name is "event" or "object")
        {
            Name = $"_{Name}";
        }
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

    private string GetSignatureExpression(List<TypeDef>? genericTypeParametersOverride)
    {
        TypeDef type = Type.OverrideGenericTypeParameter(genericTypeParametersOverride);
        return $"{type.GetTypeExpression()} {Name}";
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

    public static string GetParametersSignatureExpression(List<ParameterDef> parameters, List<TypeDef>? genericTypeParametersOverride)
    {
        if (parameters.Count == 0)
            return "";

        StringBuilder builder = new();
        string commaSpace = "";
        foreach (var parameter in parameters)
        {
            _ = builder.Append($"{commaSpace}{parameter.GetSignatureExpression(genericTypeParametersOverride)}");
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
