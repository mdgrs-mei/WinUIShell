using System.Text;
using WinUIShell.Server;

namespace WinUIShell.Generator;

internal class MethodDef
{
    private readonly Api.MethodDef _apiMethodDef;
    private readonly ObjectDef _objectDef;
    private readonly TypeDef? _explicitInterfaceType;
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
        _explicitInterfaceType = apiMethodDef.ExplicitInterfaceType is null ? null : new TypeDef(apiMethodDef.ExplicitInterfaceType);
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
        return $"{interfaceTypeName}{_apiMethodDef.Name}";
    }

    public string GetSignatureExpression()
    {
        string newExpression = _apiMethodDef.HidesBase ? "new " : "";
        string overrideExpression = _apiMethodDef.IsOverride ? "override " : "";
        return $"{newExpression}{overrideExpression}{ReturnType!.GetTypeExpression()} {GetName()}({GetParametersExpression()})";
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
