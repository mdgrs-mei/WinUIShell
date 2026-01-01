
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

        var obj = Activator.CreateInstance(
            type,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            arguments,
            null);

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
                    // Return the first match.
                    // This is not precise if there are multiple overloads with the same parameter count.
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

        SetPropertyOrField(obj, obj.GetType(), propertyName, value);
    }

    public void SetStaticProperty(string className, string propertyName, object? value)
    {
        var classType = Type.GetType(className);
        if (classType == null)
        {
            throw new InvalidOperationException($"Type [{className}] not found.");
        }
        SetPropertyOrField(null, classType, propertyName, value);
    }

    public object? GetProperty(object obj, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(obj);
        if (!IsValid(obj))
            return null;

        return GetPropertyOrField(obj, obj.GetType(), propertyName);
    }

    public object? GetStaticProperty(string className, string propertyName)
    {
        var classType = Type.GetType(className);
        if (classType == null)
        {
            throw new InvalidOperationException($"Type [{className}] not found.");
        }
        return GetPropertyOrField(null, classType, propertyName);
    }

    private void SetPropertyOrField(object? obj, Type objType, string name, object? value)
    {
        BindingFlags instanceOrStatic = obj is null ? BindingFlags.Static : BindingFlags.Instance;
        var property = objType.GetProperty(name, instanceOrStatic | BindingFlags.Public | BindingFlags.NonPublic);
        if (property is not null)
        {
            property.SetValue(obj, value);
            return;
        }

        var field = objType.GetField(name, instanceOrStatic | BindingFlags.Public | BindingFlags.NonPublic);
        if (field is not null)
        {
            field.SetValue(obj, value);
            return;
        }
        throw new InvalidOperationException($"Property or Filed [{name}] not found.");
    }

    private object? GetPropertyOrField(object? obj, Type objType, string name)
    {
        BindingFlags instanceOrStatic = obj is null ? BindingFlags.Static : BindingFlags.Instance;
        var property = objType.GetProperty(name, instanceOrStatic | BindingFlags.Public | BindingFlags.NonPublic);
        if (property is not null)
        {
            return property.GetValue(obj);
        }

        var field = objType.GetField(name, instanceOrStatic | BindingFlags.Public | BindingFlags.NonPublic);
        if (field is not null)
        {
            return field.GetValue(obj);
        }
        throw new InvalidOperationException($"Property or Filed [{name}] not found.");
    }

    public void SetIndexerProperty(object obj, object? value, object?[] indexArguments)
    {
        ArgumentNullException.ThrowIfNull(obj);
        if (!IsValid(obj))
            return;

        var type = obj.GetType();
        var property = GetIndexerPropertyInfo(type, indexArguments);

        if (property == null)
        {
            throw new InvalidOperationException($"Indexer property for [{obj}] not found.");
        }
        property.SetValue(obj, value, indexArguments);
    }

    public object? GetIndexerProperty(object obj, object?[] indexArguments)
    {
        ArgumentNullException.ThrowIfNull(obj);
        if (!IsValid(obj))
            return null;

        var type = obj.GetType();
        var property = GetIndexerPropertyInfo(type, indexArguments);

        if (property == null)
        {
            throw new InvalidOperationException($"Indexer property for [{obj}] not found.");
        }
        return property.GetValue(obj, indexArguments);
    }

    private PropertyInfo? GetIndexerPropertyInfo(
        Type objType,
        object?[]? indexArguments)
    {
        var indexerName = objType.GetCustomAttribute<DefaultMemberAttribute>()!.MemberName;

        if (indexArguments is null)
        {
            return objType.GetProperty(indexerName, Type.EmptyTypes);
        }
        else
        if (indexArguments.Contains(null))
        {
            var properties = objType.GetProperties();
            foreach (var property in properties)
            {
                if (property.Name != indexerName)
                    continue;

                int parameterCount = property.GetIndexParameters().Length;
                if (parameterCount == indexArguments.Length)
                {
                    // Return the first match.
                    // This is not precise if there are multiple overloads with the same parameter count.
                    return property;
                }
            }
            return null;
        }
        else
        {
            Type[] types = [.. indexArguments.Select(argument => argument is not null ? argument.GetType() : typeof(object))];
            return objType.GetProperty(indexerName, types);
        }
    }
}
