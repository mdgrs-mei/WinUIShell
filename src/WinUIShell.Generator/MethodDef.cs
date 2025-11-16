using System.Text;
using WinUIShell.Server;

namespace WinUIShell.Generator;

internal class MethodDef
{
    private readonly Api.MethodDef _apiMethodDef;
    private readonly ObjectDef _objectDef;
    private readonly MemberDefType _memberDefType;
    private readonly TypeDef? _explicitInterfaceType;
    private readonly bool _isUnsafe;
    public TypeDef? ReturnType { get; }

    private static readonly List<string> _unsupportedMethodNames =
    [
        "Equals",
        "GetHashCode",
        "GetType",
    ];

    public MethodDef(
        Api.MethodDef apiMethodDef,
        ObjectDef objectDef,
        MemberDefType memberDefType)
    {
        _apiMethodDef = apiMethodDef;
        _objectDef = objectDef;
        _memberDefType = memberDefType;
        _explicitInterfaceType = apiMethodDef.ExplicitInterfaceType is null ? null : new TypeDef(apiMethodDef.ExplicitInterfaceType);

        if (apiMethodDef.ReturnType is not null)
        {
            ReturnType = new TypeDef(apiMethodDef.ReturnType);
            if (ReturnType.IsPointer)
            {
                _isUnsafe = true;
            }
        }

        foreach (var parameter in _apiMethodDef.Parameters)
        {
            var parameterType = new TypeDef(parameter.Type);
            if (parameterType.IsPointer)
            {
                _isUnsafe = true;
            }
        }
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
        string unsafeExpression = _isUnsafe ? "unsafe " : "";
        string accessorExpression = (_objectDef.Type.IsInterface || _explicitInterfaceType is not null) ? "" : "public ";
        string staticExpression = _memberDefType == MemberDefType.Static ? "static " : "";
        string newExpression = _apiMethodDef.HidesBase ? "new " : "";
        string overrideExpression = _apiMethodDef.IsOverride ? "override " : "";
        return $"{unsafeExpression}{accessorExpression}{staticExpression}{newExpression}{overrideExpression}{ReturnType!.GetTypeExpression()} {GetName()}({GetParametersExpression()})";
    }

    public string GetConstructorSignatureExpression(string className)
    {
        string unsafeExpression = _isUnsafe ? "unsafe " : "";
        string accessorExpression = _objectDef.Type.IsInterface ? "" : "public ";
        return $"{unsafeExpression}{accessorExpression}{className}({GetParametersExpression()})";
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
