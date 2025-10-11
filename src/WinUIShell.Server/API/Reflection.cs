using System.Reflection;

namespace WinUIShell.Server;

internal static class Reflection
{
    public static bool IsNullable(PropertyInfo propertyInfo)
    {
        var nullabilityInfoContext = new NullabilityInfoContext();
        var nullability = nullabilityInfoContext.Create(propertyInfo);
        if (nullability.WriteState == NullabilityState.Nullable || nullability.ReadState == NullabilityState.Nullable)
        {
            return true;
        }

        return false;
    }

    public static bool IsNullable(ParameterInfo parameterInfo)
    {
        var nullabilityInfoContext = new NullabilityInfoContext();
        var nullability = nullabilityInfoContext.Create(parameterInfo);
        if (nullability.WriteState == NullabilityState.Nullable || nullability.ReadState == NullabilityState.Nullable)
        {
            return true;
        }

        return false;
    }
}
