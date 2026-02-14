using System.Reflection;

namespace WinUIShell.ApiExporter;

internal static class Reflection
{
    public static bool IsNullable(PropertyInfo propertyInfo)
    {
        if (propertyInfo.PropertyType.IsGenericParameter)
            return false;

        var nullabilityInfoContext = new NullabilityInfoContext();
        var nullability = nullabilityInfoContext.Create(propertyInfo);
        if (nullability.WriteState == NullabilityState.Nullable || nullability.ReadState == NullabilityState.Nullable)
        {
            return true;
        }

        return false;
    }

    public static bool IsNullable(FieldInfo fieldInfo)
    {
        if (fieldInfo.FieldType.IsGenericParameter)
            return false;

        var nullabilityInfoContext = new NullabilityInfoContext();
        var nullability = nullabilityInfoContext.Create(fieldInfo);
        if (nullability.WriteState == NullabilityState.Nullable || nullability.ReadState == NullabilityState.Nullable)
        {
            return true;
        }

        return false;
    }

    public static bool IsNullable(ParameterInfo parameterInfo)
    {
        if (parameterInfo.ParameterType.IsGenericParameter)
            return false;

        var nullabilityInfoContext = new NullabilityInfoContext();
        var nullability = nullabilityInfoContext.Create(parameterInfo);
        if (nullability.WriteState == NullabilityState.Nullable || nullability.ReadState == NullabilityState.Nullable)
        {
            return true;
        }

        return false;
    }

    public static bool IsNullable(Type type)
    {
        if (type.IsGenericParameter)
            return false;

        return Nullable.GetUnderlyingType(type) is not null;
    }
}
