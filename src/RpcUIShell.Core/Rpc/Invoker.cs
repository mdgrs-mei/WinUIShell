using System.Reflection;

namespace RpcUIShell.Core;

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
        return CreateObject(type, arguments);
    }

    internal object CreateObject(Type type, object?[]? arguments = null)
    {
        if (type.IsInterface)
        {
            var interfaceImplType = GetInterfaceImplType(type);
            if (interfaceImplType is null)
            {
                throw new InvalidOperationException($"Unsupported interface type [{type.FullName}].");
            }
            type = interfaceImplType;
        }

        var obj = Activator.CreateInstance(
            type,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            arguments,
            null);

        if (obj is null)
        {
            throw new InvalidOperationException($"Failed to create instance of type [{type.FullName}].");
        }
        return obj;
    }

    private static Type? GetInterfaceImplType(Type interfaceType)
    {
        // Get interface Impl type fullname from interface type fullname.
        // fullName has a format like "clientAssemblyName.Namespace.Class`1+InnerClass+InnerMost`2[[clientAssemblyName.Namespace.GenericArgumentClass, clientAssemblyName, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]".

        var clientAssemblyName = TypeMapping.Get().ClientNamespace;
        var interfaceFullName = interfaceType.FullName!;

        // System interface types don't have "clientAssemblyName" namespace. Add it here as Impl classes are always under "clientAssemblyName" namespace.
        if (!interfaceFullName.StartsWith($"{clientAssemblyName}.", StringComparison.Ordinal))
        {
            interfaceFullName = $"{clientAssemblyName}." + interfaceFullName;
        }

        int insertIndex = interfaceFullName.Length;
        int firstGenericArgumentSeparator = interfaceFullName.IndexOf('[', StringComparison.Ordinal);
        if (firstGenericArgumentSeparator >= 0)
        {
            insertIndex = firstGenericArgumentSeparator;
        }

        int lastNestedClassSeparator = interfaceFullName.LastIndexOf('+', insertIndex - 1);
        int lastGenericTypeSeparator = interfaceFullName.LastIndexOf('`', insertIndex - 1);
        if (lastNestedClassSeparator >= 0)
        {
            if (lastNestedClassSeparator < lastGenericTypeSeparator)
            {
                insertIndex = lastGenericTypeSeparator;
            }
        }
        else if (lastGenericTypeSeparator >= 0)
        {
            insertIndex = lastGenericTypeSeparator;
        }

        string implTypeFullName = $"{interfaceFullName.Insert(insertIndex, "_Impl")}, {clientAssemblyName}";
        return Type.GetType(implTypeFullName);
    }

    public object? InvokeMethod(object obj, string? typeName, string methodName, object?[]? arguments = null)
    {
        ArgumentNullException.ThrowIfNull(obj);
        if (!IsValid(obj))
            return null;

        Type type = typeName is null ? obj.GetType() : Type.GetType(typeName)!;
        MethodInfo? method = GetMethod(type, methodName, arguments,
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
        else if (arguments.Contains(null))
        {
            var methods = objType.GetMethods(bindingFlags);
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

    public void SetProperty(object obj, string? typeName, string propertyName, object? value)
    {
        ArgumentNullException.ThrowIfNull(obj);
        if (!IsValid(obj))
            return;

        Type type = typeName is null ? obj.GetType() : Type.GetType(typeName)!;
        SetPropertyOrField(obj, type, propertyName, value);
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

    public object? GetProperty(object obj, string? typeName, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(obj);
        if (!IsValid(obj))
            return null;

        Type type = typeName is null ? obj.GetType() : Type.GetType(typeName)!;
        return GetPropertyOrField(obj, type, propertyName);
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
        PropertyInfo? property = null;
        try
        {
            property = objType.GetProperty(name, instanceOrStatic | BindingFlags.Public | BindingFlags.NonPublic);
        }
        catch (AmbiguousMatchException)
        {
            // If multiple matches are found, search the hierarchy.
            Type? type = objType;
            while (type is not null)
            {
                var properties = objType.GetProperties(instanceOrStatic | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                foreach (var p in properties)
                {
                    if (p.Name == name)
                    {
                        property = p;
                        break;
                    }
                }

                if (property is not null)
                {
                    break;
                }
                else
                {
                    type = type.BaseType;
                }
            }
        }

        if (property is not null)
        {
            return property.GetValue(obj);
        }

        FieldInfo? field = null;
        try
        {
            field = objType.GetField(name, instanceOrStatic | BindingFlags.Public | BindingFlags.NonPublic);
        }
        catch (AmbiguousMatchException)
        {
            // If multiple matches are found, search the hierarchy.
            Type? type = objType;
            while (type is not null)
            {
                var fields = objType.GetFields(instanceOrStatic | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                foreach (var f in fields)
                {
                    if (f.Name == name)
                    {
                        field = f;
                        break;
                    }
                }

                if (field is not null)
                {
                    break;
                }
                else
                {
                    type = type.BaseType;
                }
            }
        }

        if (field is not null)
        {
            return field.GetValue(obj);
        }
        throw new InvalidOperationException($"Property or Filed [{name}] not found.");
    }

    public void SetIndexerProperty(object obj, string? typeName, string indexerName, object? value, object?[] indexArguments)
    {
        ArgumentNullException.ThrowIfNull(obj);
        if (!IsValid(obj))
            return;

        Type type = typeName is null ? obj.GetType() : Type.GetType(typeName)!;
        var property = GetIndexerPropertyInfo(type, indexerName, indexArguments);

        if (property == null)
        {
            throw new InvalidOperationException($"Indexer property for [{obj}] not found.");
        }
        property.SetValue(obj, value, indexArguments);
    }

    public object? GetIndexerProperty(object obj, string? typeName, string indexerName, object?[] indexArguments)
    {
        ArgumentNullException.ThrowIfNull(obj);
        if (!IsValid(obj))
            return null;

        Type type = typeName is null ? obj.GetType() : Type.GetType(typeName)!;
        var property = GetIndexerPropertyInfo(type, indexerName, indexArguments);

        if (property == null)
        {
            throw new InvalidOperationException($"Indexer property for [{obj}] not found.");
        }
        return property.GetValue(obj, indexArguments);
    }

    private PropertyInfo? GetIndexerPropertyInfo(
        Type objType,
        string indexerName,
        object?[]? indexArguments)
    {
        var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        if (indexArguments is null)
        {
            return objType.GetProperty(
                indexerName,
                bindingFlags,
                null,
                null,
                Type.EmptyTypes,
                null);
        }
        else if (indexArguments.Contains(null))
        {
            var properties = objType.GetProperties(bindingFlags);
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
            return objType.GetProperty(
                indexerName,
                bindingFlags,
                null,
                null,
                types,
                null);
        }
    }
}
