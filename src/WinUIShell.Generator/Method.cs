using System.Text;
using WinUIShell.Server;

namespace WinUIShell.Generator;

internal class Method
{
    private readonly Api.MethodDef _methodDef;
    private readonly Api.ObjectDef _objectDef;
    public ArgumentType? ReturnType { get; }

    private static readonly List<string> _unsupportedMethodNames =
    [
        "Equals",
        "GetHashCode",
        "GetType",
    ];

    public Method(Api.MethodDef methodDef, Api.ObjectDef objectDef)
    {
        _methodDef = methodDef;
        _objectDef = objectDef;
        ReturnType = methodDef.ReturnType is null ? null : new ArgumentType(methodDef.ReturnType);
    }

    public bool IsSupported()
    {
        if (_methodDef.IsGenericMethod)
            return false;

        if (!string.IsNullOrEmpty(_methodDef.Name))
        {
            if (_unsupportedMethodNames.Contains(_methodDef.Name!))
                return false;
        }

        if (ReturnType is not null)
        {
            if (!ReturnType.IsSupported())
                return false;
        }

        foreach (var parameter in _methodDef.Parameters)
        {
            var parameterType = new ArgumentType(parameter.Type);
            if (!parameterType.IsSupported())
                return false;
        }
        return true;
    }

    public string GetName()
    {
        return _methodDef.Name!;
    }

    public string GetSignatureExpression()
    {
        string overrideExpression = (_methodDef.IsVirtual && !_objectDef.Type.IsInterface) ? "override " : "";
        return $"{overrideExpression}{ReturnType!.GetTypeExpression()} {GetName()}({GetParametersExpression()})";
    }

    public string GetParametersExpression()
    {
        if (_methodDef.Parameters.Count == 0)
            return "";

        StringBuilder builder = new();
        string commaSpace = "";
        foreach (var parameter in _methodDef.Parameters)
        {
            var parameterType = new ArgumentType(parameter.Type);
            _ = builder.Append($"{commaSpace}{parameterType.GetTypeExpression()} {parameter.Name}");
            commaSpace = ", ";
        }
        return builder.ToString();
    }

    public string GetArgumentsExpression()
    {
        if (_methodDef.Parameters.Count == 0)
            return "";

        StringBuilder builder = new();
        foreach (var parameter in _methodDef.Parameters)
        {
            var argumentType = new ArgumentType(parameter.Type);
            _ = builder.Append($", {argumentType.GetArgumentExpression(parameter.Name!)}");
        }
        return builder.ToString();
    }
}
