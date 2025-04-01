
using System.Reflection;

namespace WinUIShell.Common;

public class Invoker : Singleton<Invoker>
{
    public delegate bool ValidateCallback(object obj);
    public ValidateCallback? Validator { get; set; }

    private bool IsValid(object obj)
    {
        if (Validator is null)
            return true;

        return Validator(obj);
    }

    public object CreateObject(string typeName, object?[]? arguments = null)
    {
        var type = Type.GetType(typeName);
        if (type == null)
        {
            throw new InvalidOperationException($"Type [{typeName}] not found.");
        }
        var obj = Activator.CreateInstance(type, arguments);
        if (obj == null)
        {
            throw new InvalidOperationException($"Failed to create instance of type [{typeName}].");
        }
        return obj;
    }

    public object? InvokeMethod(object obj, string methodName, object?[]? arguments = null)
    {
        ArgumentNullException.ThrowIfNull(obj);
        if (!IsValid(obj))
            return null;

        MethodInfo? method = GetMethod(obj.GetType(), methodName, arguments,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (method == null)
        {
            throw new InvalidOperationException($"Method [{methodName}] not found.");
        }
        return method.Invoke(obj, arguments);
    }

    public object? InvokeStaticMethod(string className, string methodName, object?[]? arguments = null)
    {
        var classType = Type.GetType(className);
        if (classType == null)
        {
            throw new InvalidOperationException($"Type [{className}] not found.");
        }

        var method = GetMethod(classType, methodName, arguments,
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        if (method == null)
        {
            throw new InvalidOperationException($"Method [{methodName}] not found.");
        }
        return method.Invoke(null, arguments);
    }

    private MethodInfo? GetMethod(
        Type objType,
        string methodName,
        object?[]? arguments,
        BindingFlags bindingFlags)
    {
        if (arguments is null)
        {
            return objType.GetMethod(methodName, bindingFlags, Type.EmptyTypes);
        }
        else
        if (arguments.Contains(null))
        {
            var methods = objType.GetMethods();
            foreach (var method in methods)
            {
                if (method.Name != methodName)
                    continue;

                int parameterCount = method.GetParameters().Length;
                if (parameterCount == arguments.Length)
                {
                    return method;
                }
            }
            return null;
        }
        else
        {
            Type[] types = [.. arguments.Select(argument => argument is not null ? argument.GetType() : typeof(object))];
            return objType.GetMethod(methodName, bindingFlags, types);
        }
    }

    public void SetProperty(object obj, string propertyName, object? value)
    {
        ArgumentNullException.ThrowIfNull(obj);
        if (!IsValid(obj))
            return;

        var property = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (property == null)
        {
            throw new InvalidOperationException($"Property [{propertyName}] not found.");
        }
        property.SetValue(obj, value);
    }

    public object? GetProperty(object obj, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(obj);
        if (!IsValid(obj))
            return null;

        var property = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (property == null)
        {
            throw new InvalidOperationException($"Property [{propertyName}] not found.");
        }
        return property.GetValue(obj);
    }

    public object? GetStaticProperty(string className, string propertyName)
    {
        var classType = Type.GetType(className);
        if (classType == null)
        {
            throw new InvalidOperationException($"Type [{className}] not found.");
        }

        var property = classType.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (property == null)
        {
            throw new InvalidOperationException($"Property [{propertyName}] not found.");
        }
        return property.GetValue(null);
    }

    public void SetIndexerProperty(object obj, object index, object? value)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentNullException.ThrowIfNull(index);
        if (!IsValid(obj))
            return;

        var type = obj.GetType();
        var indexType = index.GetType();
        var property = type.GetProperty(
            type.GetCustomAttribute<DefaultMemberAttribute>()!.MemberName,
            [indexType]);

        if (property == null)
        {
            throw new InvalidOperationException($"Indexer property for [{obj}] not found.");
        }
        property.SetValue(obj, value, [index]);
    }

    public object? GetIndexerProperty(object obj, object index)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentNullException.ThrowIfNull(index);
        if (!IsValid(obj))
            return null;

        var type = obj.GetType();
        var indexType = index.GetType();
        var property = type.GetProperty(
            type.GetCustomAttribute<DefaultMemberAttribute>()!.MemberName,
            [indexType]);

        if (property == null)
        {
            throw new InvalidOperationException($"Indexer property for [{obj}] not found.");
        }
        return property.GetValue(obj, [index]);
    }
}
