using System.Text;
using WinUIShell.Server;

namespace WinUIShell.Generator;

internal class MethodDef
{
    private readonly Api.MethodDef _apiMethodDef;
    private readonly ObjectDef _objectDef;
    public TypeDef? ReturnType { get; }

    private static readonly List<string> _unsupportedMethodNames =
    [
        "Equals",
        "GetHashCode",
        "GetType",
    ];

    public MethodDef(Api.MethodDef apiMethodDef, ObjectDef objectDef)
    {
        _apiMethodDef = apiMethodDef;
        _objectDef = objectDef;
        ReturnType = apiMethodDef.ReturnType is null ? null : new TypeDef(apiMethodDef.ReturnType);
    }

    public bool IsSupported()
    {
        if (_apiMethodDef.IsGenericMethod)
            return false;

        if (!string.IsNullOrEmpty(_apiMethodDef.Name))
        {
            if (_unsupportedMethodNames.Contains(_apiMethodDef.Name!))
                return false;
        }

        if (ReturnType is not null)
        {
            if (!ReturnType.IsSupported())
                return false;
        }

        foreach (var parameter in _apiMethodDef.Parameters)
        {
            var parameterType = new TypeDef(parameter.Type);
            if (!parameterType.IsSupported())
                return false;
        }
        return true;
    }

    public string GetName()
    {
        return _apiMethodDef.Name!;
    }

    public string GetSignatureExpression()
    {
        string overrideExpression = (_apiMethodDef.IsVirtual && !_objectDef.Type.IsInterface) ? "override " : "";
        return $"{overrideExpression}{ReturnType!.GetTypeExpression()} {GetName()}({GetParametersExpression()})";
    }

    public string GetParametersExpression()
    {
        if (_apiMethodDef.Parameters.Count == 0)
            return "";

        StringBuilder builder = new();
        string commaSpace = "";
        foreach (var parameter in _apiMethodDef.Parameters)
        {
            var parameterType = new TypeDef(parameter.Type);
            _ = builder.Append($"{commaSpace}{parameterType.GetTypeExpression()} {parameter.Name}");
            commaSpace = ", ";
        }
        return builder.ToString();
    }

    public string GetArgumentsExpression()
    {
        if (_apiMethodDef.Parameters.Count == 0)
            return "";

        StringBuilder builder = new();
        foreach (var parameter in _apiMethodDef.Parameters)
        {
            var typeDef = new TypeDef(parameter.Type);
            _ = builder.Append($", {typeDef.GetArgumentExpression(parameter.Name!)}");
        }
        return builder.ToString();
    }
}
