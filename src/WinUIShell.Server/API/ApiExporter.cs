using System.Diagnostics;
using System.Reflection;
using System.Xml.Serialization;
using WinUIShell.Common;

namespace WinUIShell.Server;

public class ApiExporter : Singleton<ApiExporter>
{
    private readonly Api _api = new();
    private readonly HashSet<string> _addedObjects = [];

    public void Export(string apiFilePath)
    {
        AddEnums();
        AddObjects();
        ExportToFile(apiFilePath);
    }

    private void AddEnums()
    {
        AddEnumsInAssembly(typeof(Microsoft.UI.Xaml.Controls.BackgroundSizing)); // Microsoft.WinUI
        AddEnumsInAssembly(typeof(Microsoft.UI.Windowing.CompactOverlaySize)); // Microsoft.InteractiveExperiences.Projection
        AddEnumsInAssembly(typeof(Windows.UI.Text.FontStretch)); // Microsoft.Windows.SDK.NET
        AddEnum(typeof(EventCallbackRunspaceMode));
        AddEnum(typeof(UriHostNameType));
        AddEnum(typeof(UriKind));
        AddEnum(typeof(UriComponents));
        AddEnum(typeof(UriFormat));
        AddEnum(typeof(UriPartial));
        AddEnum(typeof(StringComparison));
    }

    private void AddEnumsInAssembly(Type representativeTypeInAssembly)
    {
        var assembly = representativeTypeInAssembly.Assembly;
        var enumTypes = assembly.GetTypes().Where(type => type.IsEnum).OrderBy(type => type.FullName);

        foreach (var enumType in enumTypes)
        {
            AddEnum(enumType);
        }
    }

    private void AddEnum(Type enumType)
    {
        var assembly = enumType.Assembly;
        var underlyingType = Enum.GetUnderlyingType(enumType);

        var def = new Api.EnumDef
        {
            Name = enumType.Name,
            FullName = $"{enumType.FullName}, {assembly.GetName().Name}",
            Namespace = enumType.Namespace!,
            UnderlyingType = underlyingType.Name
        };

        foreach (var entryName in Enum.GetNames(enumType))
        {
            var value = Enum.Parse(enumType, entryName);
            var underlyingTypeValue = Convert.ChangeType(value, underlyingType);
            def.Entries.Add(new Api.EnumEntryDef
            {
                Name = entryName,
                Value = underlyingTypeValue
            });
        }

        _api.Enums.Add(def);
    }

    private void AddObjects()
    {
        AddObject(typeof(Microsoft.UI.Xaml.Application));
        AddObject(typeof(Microsoft.UI.Xaml.DebugSettings));
        AddObject(typeof(Microsoft.UI.Xaml.ResourceDictionary));
        AddObject(typeof(Uri));
        AddObject(typeof(UriCreationOptions));
        AddObject(typeof(Windows.UI.Core.CoreDispatcher));
        AddObject(typeof(Microsoft.UI.Dispatching.DispatcherQueue));
        AddObject(typeof(KeyValuePair<,>));
        AddObject(typeof(System.Collections.IEnumerator));
        AddObject(typeof(System.Collections.IEnumerable));
        AddObject(typeof(IEnumerable<>));
        AddObject(typeof(IEnumerator<>));
        AddObject(typeof(IDictionary<,>));
        AddObject(typeof(ICollection<>));
        AddObject(typeof(IList<>));
        AddObject(typeof(Span<>));
        AddObject(typeof(Microsoft.UI.Dispatching.DispatcherQueueTimer));
        AddObject(typeof(Microsoft.UI.Dispatching.DispatcherExitDeferral));
        AddObject(typeof(Microsoft.UI.Xaml.LaunchActivatedEventArgs));
        AddObject(typeof(Windows.UI.Core.ICoreAcceleratorKeys));
        AddObject(typeof(System.TimeSpan));
        AddObject(typeof(System.ReadOnlySpan<>));
        AddObject(typeof(Windows.ApplicationModel.Activation.LaunchActivatedEventArgs));
    }

    private void AddObject(Type type)
    {
        if (type.IsEnum)
            return;

        if (type.IsNested)
        {
            AddObject(type.DeclaringType!);
            return;
        }

        string fullName = GetObjectFullName(type);
        if (_addedObjects.Contains(fullName))
            return;
        _ = _addedObjects.Add(fullName);

        var def = GetObjectDef(type);

        _api.Objects.Add(def);
    }

    private Api.ObjectDef GetObjectDef(Type type)
    {
        var def = new Api.ObjectDef
        {
            Name = GetObjectTypeName(type),
            FullName = GetObjectFullName(type),
            Namespace = type.Namespace!,
            Type = GetTypeDef(type),
        };

        if (type.BaseType != typeof(object) &&
            type.BaseType != typeof(ValueType) &&
            type.BaseType is not null)
        {
            def.BaseType = GetTypeDef(type.BaseType);
        }

        foreach (var interfaceType in type.GetInterfaces())
        {
            def.Interfaces.Add(GetTypeDef(interfaceType));
        }

        foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Static))
        {
            def.StaticProperties.Add(GetPropertyDef(propertyInfo));
        }
        foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            def.InstanceProperties.Add(GetPropertyDef(propertyInfo));
        }

        foreach (var constructor in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
        {
            def.Constructors.Add(GetConstructorDef(constructor));
        }

        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            if (IsIgnoredMethod(method))
                continue;
            def.StaticMethods.Add(GetMethodDef(method));
        }

        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
        {
            if (IsIgnoredMethod(method))
                continue;
            def.InstanceMethods.Add(GetMethodDef(method));
        }
        // Explicit interface implementations.
        foreach (var method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            if (!method.IsFinal || !method.IsPrivate)
                continue;

            if (IsIgnoredMethod(method))
                continue;

            def.InstanceMethods.Add(GetMethodDef(method));
        }

        foreach (var nestedType in type.GetNestedTypes())
        {
            def.NestedTypes.Add(GetObjectDef(nestedType));
        }

        return def;
    }

    private Api.PropertyDef GetPropertyDef(PropertyInfo propertyInfo)
    {
        var propertyType = propertyInfo.PropertyType;
        var typeDef = GetTypeDef(propertyType);
        typeDef.IsNullable = Reflection.IsNullable(propertyInfo);

        return new Api.PropertyDef
        {
            Name = propertyInfo.Name,
            Type = typeDef,
            CanRead = propertyInfo.CanRead,
            CanWrite = propertyInfo.CanWrite,
            HidesBase = HidesBaseMethod(propertyInfo.GetMethod),
        };
    }

    private Api.MethodDef GetConstructorDef(ConstructorInfo constructorInfo)
    {
        var methodDef = new Api.MethodDef();
        var parameters = constructorInfo.GetParameters();
        foreach (var parameter in parameters)
        {
            methodDef.Parameters.Add(GetParameterDef(parameter));
        }
        return methodDef;
    }

    private Api.MethodDef GetMethodDef(MethodInfo methodInfo)
    {
        var returnParameter = methodInfo.ReturnParameter;
        var returnType = GetTypeDef(methodInfo.ReturnType);
        returnType.IsNullable = Reflection.IsNullable(returnParameter);
        returnType.IsIn = returnParameter.IsIn;
        returnType.IsOut = returnParameter.IsOut;

        var methodDef = new Api.MethodDef
        {
            Name = GetMethodName(methodInfo),
            ReturnType = returnType,
            ExplicitInterfaceType = GetExplicitInterfaceType(methodInfo),
            IsGenericMethod = methodInfo.IsGenericMethod,
            IsOverride = IsOverride(methodInfo),
            HidesBase = HidesBaseMethod(methodInfo),
        };

        var parameters = methodInfo.GetParameters();
        foreach (var parameter in parameters)
        {
            methodDef.Parameters.Add(GetParameterDef(parameter));
        }
        return methodDef;
    }

    private string GetMethodName(MethodInfo methodInfo)
    {
        var name = methodInfo.Name;
        int dot = name.LastIndexOf('.');
        return name[(dot + 1)..];
    }

    private bool IsOverride(MethodInfo methodInfo)
    {
        Type objectType = methodInfo.ReflectedType!;
        bool isImplemented = methodInfo.DeclaringType == objectType;
        if (!isImplemented)
            return false;

        if (!methodInfo.IsVirtual)
            return false;

        if (objectType.IsInterface)
            return false;

        bool isNewSlot = (methodInfo.Attributes & MethodAttributes.NewSlot) > 0;
        return !isNewSlot;
    }

    private bool HidesBaseMethod(MethodInfo? methodInfo)
    {
        if (methodInfo is null)
            return false;

        Type objectType = methodInfo.ReflectedType!;
        bool isImplemented = methodInfo.DeclaringType == objectType;
        if (!isImplemented)
            return false;

        if (methodInfo.IsVirtual)
        {
            bool isNewSlot = (methodInfo.Attributes & MethodAttributes.NewSlot) > 0;
            if (!isNewSlot)
                // Override.
                return false;

            if (objectType.BaseType is not null && HasMethod(objectType.BaseType, methodInfo))
                return true;

            foreach (var interfaceType in objectType.GetInterfaces())
            {
                if (HasMethod(interfaceType, methodInfo))
                    return objectType.IsInterface;
            }
        }
        else
        {
            if (objectType.BaseType is not null && HasMethod(objectType.BaseType, methodInfo))
                return true;

            foreach (var interfaceType in objectType.GetInterfaces())
            {
                if (HasMethod(interfaceType, methodInfo))
                    return true;
            }
        }
        return false;
    }

    private bool HasMethod(Type type, MethodInfo methodInfo)
    {
        var name = GetMethodName(methodInfo);
        var parameterTypes = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
        var method = type.GetMethod(
            name,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.ExactBinding,
            parameterTypes);
        return method is not null;
    }

    private Api.TypeDef? GetExplicitInterfaceType(MethodInfo methodInfo)
    {
        if (!methodInfo.IsFinal || !methodInfo.IsPrivate)
            return null;

        Type objectType = methodInfo.DeclaringType!;
        if (objectType.IsInterface)
        {
            foreach (var interfaceType in objectType.GetInterfaces())
            {
                if (HasMethod(interfaceType, methodInfo))
                    return GetTypeDef(interfaceType);
            }
        }
        else
        {
            foreach (var interfaceType in objectType.GetInterfaces())
            {
                var interfaceMap = objectType.GetInterfaceMap(interfaceType);
                if (interfaceMap.TargetMethods.Contains(methodInfo))
                    return GetTypeDef(interfaceType);
            }
        }

        Debug.Assert(false, "Could not find interface type of explicit implementation.");
        return null;
    }

    private Api.ParameterDef GetParameterDef(ParameterInfo parameterInfo)
    {
        var type = parameterInfo.ParameterType;
        var typeDef = GetTypeDef(type);
        typeDef.IsNullable = Reflection.IsNullable(parameterInfo);
        typeDef.IsIn = parameterInfo.IsIn;
        typeDef.IsOut = parameterInfo.IsOut;

        return new Api.ParameterDef
        {
            Name = parameterInfo.Name,
            Type = typeDef,
        };
    }

    private Api.TypeDef GetTypeDef(Type type)
    {
        var name = GetTypeDefName(type);
        var typeDef = new Api.TypeDef
        {
            Name = name,
            IsNullable = Reflection.IsNullable(type),
            IsEnum = type.IsEnum,
            IsArray = type.IsArray,
            IsDelegate = typeof(Delegate).IsAssignableFrom(type),
            IsPointer = type.IsPointer,
            IsByRef = type.IsByRef,
            IsGenericType = type.IsGenericType,
            IsGenericTypeParameter = type.IsGenericTypeParameter,
            IsGenericMethodParameter = type.IsGenericMethodParameter,
            IsInterface = type.IsInterface,
            IsSystemObject = name.StartsWith("System.", StringComparison.Ordinal),
        };

        if (type.IsByRef || type.IsArray || type.IsPointer)
        {
            var elementType = type.GetElementType();
            typeDef.ElementType = GetTypeDef(elementType!);
        }
        else
        {
            bool isUnsupportedSystemInterface = typeDef.IsInterface && typeDef.IsSystemObject && !Api.IsSupportedSystemInterface(name);
            if (!type.IsGenericParameter && !isUnsupportedSystemInterface)
            {
                if (type.IsGenericType && !type.IsGenericTypeDefinition)
                {
                    AddObject(type.GetGenericTypeDefinition());
                }
                else
                {
                    AddObject(type);
                }
            }
        }

        if (type.IsGenericType)
        {
            foreach (var genericTypeArgument in type.GetGenericArguments())
            {
                typeDef.GenericTypeArguments.Add(GetTypeDef(genericTypeArgument));
            }
        }

        return typeDef;
    }

    private string GetTypeDefName(Type type)
    {
        if (type.IsByRef)
        {
            return "ByRef";
        }
        if (type.IsArray)
        {
            return "Array";
        }
        if (type.IsPointer)
        {
            return "Pointer";
        }

        string name = type.ToString();
        name = RemoveGenericExpression(name);
        name = RemovePointerExpression(name);

        return name;
    }

    private string GetObjectTypeName(Type type)
    {
        return RemoveGenericExpression(type.Name);
    }

    private string GetObjectFullName(Type type)
    {
        var assembly = type.Assembly;
        return $"{type.FullName}, {assembly.GetName().Name}";
    }

    private string RemoveGenericExpression(string name)
    {
        var genericSeparator = name.IndexOf('`', StringComparison.Ordinal);
        if (genericSeparator >= 0)
        {
            return name[..genericSeparator];
        }
        return name;
    }

    private string RemovePointerExpression(string name)
    {
        return name.Replace("*", "", StringComparison.Ordinal);
    }

    private bool IsIgnoredMethod(MethodInfo methodInfo)
    {
        bool isImplemented = methodInfo.DeclaringType == methodInfo.ReflectedType;
        bool isHiddenMethodsLikeGetterSetter = methodInfo.IsSpecialName;
        return !isImplemented || isHiddenMethodsLikeGetterSetter;
    }

    private void ExportToFile(string filePath)
    {
        var streamWriter = new StreamWriter(filePath, append: false, System.Text.Encoding.UTF8);
        var serializer = new XmlSerializer(typeof(Api));
        serializer.Serialize(streamWriter, _api);
        streamWriter.Close();
    }
}
