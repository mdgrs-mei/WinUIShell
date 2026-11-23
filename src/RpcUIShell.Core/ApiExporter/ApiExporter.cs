using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace RpcUIShell.Core;

public class ApiExporter
{
    private readonly string _exporterAssemblyName;
    private readonly string _serverAssemblyName;
    private readonly HashSet<string> _addedTypeMappings = [];
    private readonly HashSet<string> _addedEnums = [];
    private readonly HashSet<string> _addedObjects = [];

    public Api Api { get; } = new();

    public ApiExporter(
        string exporterAssemblyName,
        string serverAssemblyName)
    {
        _exporterAssemblyName = exporterAssemblyName;
        _serverAssemblyName = serverAssemblyName;
    }

    public void Export(string apiFilePath)
    {
        AddEnum(typeof(EventCallbackRunspaceMode));
        AddObject(typeof(List<>));
        AddObject(typeof(System.Text.StringBuilder));
        AddObject(typeof(ObservableCollection<>));

        ExportToFile(apiFilePath);
    }

    public void AddTypesInAssembly(Type representativeTypeInAssembly)
    {
        ArgumentNullException.ThrowIfNull(representativeTypeInAssembly);

        var assembly = representativeTypeInAssembly.Assembly;
        var types = assembly.GetTypes().OrderBy(type => type.FullName);

        foreach (var type in types)
        {
            if (type.IsEnum)
            {
                AddEnum(type);
            }
            else
            {
                AddObject(type);
            }
        }
    }

    public void AddTypeMapping(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (!IsPublicType(type))
            return;

        if (IsIgnoredNamespace(type.Namespace))
            return;

        string fullName = GetTypeFullName(type);
        if (_addedTypeMappings.Contains(fullName))
            return;
        _ = _addedTypeMappings.Add(fullName);

        var def = new Api.TypeMappingDef
        {
            Name = GetObjectTypeName(type),
            FullName = fullName,
            Namespace = type.Namespace!,
            GenericArgumentCount = type.GetGenericArguments().Length
        };

        Api.TypeMappings.Add(def);
    }

    private void AddEnum(Type type)
    {
        if (!type.IsEnum)
            return;

        if (!IsPublicType(type))
            return;

        if (IsIgnoredNamespace(type.Namespace))
            return;

        if (type.IsNested)
        {
            AddObject(type.DeclaringType!);
            return;
        }

        string fullName = GetTypeFullName(type);
        if (_addedEnums.Contains(fullName))
            return;
        _ = _addedEnums.Add(fullName);

        var def = GetEnumDef(type);
        Api.Enums.Add(def);
    }

    private Api.EnumDef GetEnumDef(Type type)
    {
        var underlyingType = Enum.GetUnderlyingType(type);

        var def = new Api.EnumDef
        {
            Name = type.Name,
            FullName = GetTypeFullName(type),
            Namespace = type.Namespace!,
            UnderlyingType = underlyingType.Name
        };

        foreach (var entryName in Enum.GetNames(type))
        {
            var value = Enum.Parse(type, entryName);
            var underlyingTypeValue = Convert.ChangeType(value, underlyingType);
            def.Entries.Add(new Api.EnumEntryDef
            {
                Name = entryName,
                Value = underlyingTypeValue
            });
        }

        return def;
    }

    public void AddObject(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (type.IsEnum)
            return;

        if (!IsPublicType(type))
            return;

        if (IsIgnoredNamespace(type.Namespace))
            return;

        string typeDefName = GetTypeDefName(type);
        bool isSystemType = IsSystemType(typeDefName);
        bool isUnsupportedSystemInterface = type.IsInterface && isSystemType && !Api.IsSupportedSystemInterface(typeDefName);
        if (isUnsupportedSystemInterface)
            return;

        if (type.IsNested)
        {
            AddObject(type.DeclaringType!);
            return;
        }

        string fullName = GetTypeFullName(type);
        if (_addedObjects.Contains(fullName))
            return;
        _ = _addedObjects.Add(fullName);

        var def = GetObjectDef(type);

        Api.Objects.Add(def);
    }

    private bool IsPublicType(Type type)
    {
        if (type.IsGenericParameter)
            return true;

        if (type.IsNested)
        {
            return type.IsNestedPublic && IsPublicType(type.DeclaringType!);
        }
        return type.IsPublic;
    }

    private Api.ObjectDef GetObjectDef(Type type)
    {
        var def = new Api.ObjectDef
        {
            Name = GetObjectTypeName(type),
            FullName = GetTypeFullName(type),
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
            if (def.Interfaces is null)
            {
                def.Interfaces = [];
            }
            def.Interfaces.Add(GetTypeDef(interfaceType));
        }

        foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
        {
            if (def.StaticProperties is null)
            {
                def.StaticProperties = [];
            }
            def.StaticProperties.Add(GetPropertyDef(propertyInfo));
        }
        foreach (var fieldInfo in type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
        {
            if (def.StaticProperties is null)
            {
                def.StaticProperties = [];
            }
            def.StaticProperties.Add(GetPropertyDef(fieldInfo));
        }
        foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            if (def.InstanceProperties is null)
            {
                def.InstanceProperties = [];
            }
            def.InstanceProperties.Add(GetPropertyDef(propertyInfo));
        }
        foreach (var fieldInfo in type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            if (def.InstanceProperties is null)
            {
                def.InstanceProperties = [];
            }
            def.InstanceProperties.Add(GetPropertyDef(fieldInfo));
        }

        // Explicit interface implementations.
        foreach (var propertyInfo in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            var method = propertyInfo.GetMethod;
            if (method is null || !method.IsFinal || !method.IsPrivate)
                continue;

            if (GetExplicitInterfaceType(method) is null)
                continue;

            if (def.InstanceProperties is null)
            {
                def.InstanceProperties = [];
            }
            def.InstanceProperties.Add(GetPropertyDef(propertyInfo));
        }

        foreach (var constructor in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            if (def.Constructors is null)
            {
                def.Constructors = [];
            }
            def.Constructors.Add(GetConstructorDef(constructor));
        }

        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
        {
            if (IsIgnoredMethod(method))
                continue;

            if (def.StaticMethods is null)
            {
                def.StaticMethods = [];
            }
            def.StaticMethods.Add(GetMethodDef(method));
        }

        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            if (IsIgnoredMethod(method))
                continue;

            if (def.InstanceMethods is null)
            {
                def.InstanceMethods = [];
            }
            def.InstanceMethods.Add(GetMethodDef(method));
        }
        // Explicit interface implementations.
        foreach (var method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            if (!method.IsFinal || !method.IsPrivate)
                continue;

            if (IsIgnoredMethod(method))
                continue;

            if (GetExplicitInterfaceType(method) is null)
                continue;

            if (def.InstanceMethods is null)
            {
                def.InstanceMethods = [];
            }
            def.InstanceMethods.Add(GetMethodDef(method));
        }

        foreach (var eventInfo in type.GetEvents(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
        {
            if (def.StaticEvents is null)
            {
                def.StaticEvents = [];
            }
            def.StaticEvents.Add(GetEventDef(eventInfo));
        }

        foreach (var eventInfo in type.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            if (def.InstanceEvents is null)
            {
                def.InstanceEvents = [];
            }
            def.InstanceEvents.Add(GetEventDef(eventInfo));
        }
        // Explicit interface implementations.
        foreach (var eventInfo in type.GetEvents(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            var method = eventInfo.AddMethod;
            if (method is null || !method.IsFinal || !method.IsPrivate)
                continue;

            if (GetExplicitInterfaceType(method) is null)
                continue;

            if (def.InstanceEvents is null)
            {
                def.InstanceEvents = [];
            }
            def.InstanceEvents.Add(GetEventDef(eventInfo));
        }

        foreach (var nestedType in type.GetNestedTypes())
        {
            if (nestedType.IsEnum)
            {
                if (def.NestedEnums is null)
                {
                    def.NestedEnums = [];
                }
                def.NestedEnums.Add(GetEnumDef(nestedType));
            }
            else
            {
                if (def.NestedObjects is null)
                {
                    def.NestedObjects = [];
                }
                def.NestedObjects.Add(GetObjectDef(nestedType));
            }
        }

        return def;
    }

    private Api.PropertyDef GetPropertyDef(PropertyInfo propertyInfo)
    {
        var propertyType = propertyInfo.PropertyType;
        var typeDef = GetTypeDef(propertyType);
        typeDef.IsNullable = ApiReflection.IsNullable(propertyInfo);

        var propertyDef = new Api.PropertyDef
        {
            Name = GetPropertyName(propertyInfo),
            Type = typeDef,
            ExplicitInterfaceType = GetExplicitInterfaceType(propertyInfo.GetMethod),
            CanRead = propertyInfo.CanRead,
            CanWrite = propertyInfo.CanWrite,
            IsVirtual = propertyInfo.GetMethod?.IsVirtual ?? false,
            IsAbstract = propertyInfo.GetMethod?.IsAbstract ?? false,
            IsOverride = IsOverride(propertyInfo.GetMethod),
            HidesBase = HidesBaseMethod(propertyInfo.GetMethod),
            ImplementsInterface = ImplementsInterface(propertyInfo.GetMethod),
            ImplementsGlobalSystemInterface = ImplementsGlobalSystemInterface(propertyInfo.GetMethod),
        };

        var indexParameters = propertyInfo.GetIndexParameters();
        foreach (var parameter in indexParameters)
        {
            if (propertyDef.IndexParameters is null)
            {
                propertyDef.IndexParameters = [];
            }
            propertyDef.IndexParameters.Add(GetParameterDef(parameter));
        }
        return propertyDef;
    }

    private string GetPropertyName(PropertyInfo propertyInfo)
    {
        var name = propertyInfo.Name;
        int dot = name.LastIndexOf('.');
        return name[(dot + 1)..];
    }

    private string GetPropertyName(FieldInfo fieldInfo)
    {
        var name = fieldInfo.Name;
        int dot = name.LastIndexOf('.');
        return name[(dot + 1)..];
    }

    private Api.PropertyDef GetPropertyDef(FieldInfo fieldInfo)
    {
        var propertyType = fieldInfo.FieldType;
        var typeDef = GetTypeDef(propertyType);
        typeDef.IsNullable = ApiReflection.IsNullable(fieldInfo);

        var propertyDef = new Api.PropertyDef
        {
            Name = GetPropertyName(fieldInfo),
            Type = typeDef,
            ExplicitInterfaceType = null,
            CanRead = true,
            CanWrite = true,
            IsVirtual = false,
            IsAbstract = false,
            IsOverride = false,
            HidesBase = HidesBaseField(fieldInfo),
            ImplementsInterface = false,
            ImplementsGlobalSystemInterface = false,
        };

        return propertyDef;
    }

    private Api.MethodDef GetConstructorDef(ConstructorInfo constructorInfo)
    {
        var methodDef = new Api.MethodDef
        {
            Name = constructorInfo.Name,
        };

        var parameters = constructorInfo.GetParameters();
        foreach (var parameter in parameters)
        {
            if (methodDef.Parameters is null)
            {
                methodDef.Parameters = [];
            }
            methodDef.Parameters.Add(GetParameterDef(parameter));
        }
        return methodDef;
    }

    private Api.MethodDef GetMethodDef(MethodInfo methodInfo)
    {
        var returnParameter = methodInfo.ReturnParameter;
        var returnType = GetTypeDef(methodInfo.ReturnType);
        returnType.IsNullable = ApiReflection.IsNullable(returnParameter);
        returnType.IsIn = returnParameter.IsIn;
        returnType.IsOut = returnParameter.IsOut;

        var methodDef = new Api.MethodDef
        {
            Name = GetMethodName(methodInfo),
            ReturnType = returnType,
            ExplicitInterfaceType = GetExplicitInterfaceType(methodInfo),
            IsGenericMethod = methodInfo.IsGenericMethod,
            IsVirtual = methodInfo.IsVirtual,
            IsAbstract = methodInfo.IsAbstract,
            IsOverride = IsOverride(methodInfo),
            IsExtension = IsExtension(methodInfo),
            HidesBase = HidesBaseMethod(methodInfo),
            ImplementsInterface = ImplementsInterface(methodInfo),
            ImplementsGlobalSystemInterface = ImplementsGlobalSystemInterface(methodInfo),
        };

        var parameters = methodInfo.GetParameters();
        foreach (var parameter in parameters)
        {
            if (methodDef.Parameters is null)
            {
                methodDef.Parameters = [];
            }
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

    private bool IsOverride(MethodInfo? methodInfo)
    {
        if (methodInfo is null)
            return false;

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
        }

        if (objectType.BaseType is not null && HasMethod(objectType.BaseType, methodInfo))
            return true;

        if (objectType.IsInterface)
        {
            foreach (var interfaceType in objectType.GetInterfaces())
            {
                if (HasMethod(interfaceType, methodInfo))
                    return true;
            }
        }

        return false;
    }

    private bool ImplementsInterface(MethodInfo? methodInfo)
    {
        if (methodInfo is null)
            return false;

        if (!methodInfo.IsVirtual)
            return false;

        Type objectType = methodInfo.ReflectedType!;
        bool isImplemented = methodInfo.DeclaringType == objectType;
        if (!isImplemented)
            return false;

        if (objectType.IsInterface)
        {
            return true;
        }

        foreach (var interfaceType in objectType.GetInterfaces())
        {
            if (objectType.IsInterface)
            {
                if (HasMethod(interfaceType, methodInfo))
                    return true;
            }
            else
            {
                var interfaceMap = objectType.GetInterfaceMap(interfaceType);
                if (interfaceMap.TargetMethods.Contains(methodInfo))
                    return true;
            }
        }
        return false;
    }

    private bool ImplementsGlobalSystemInterface(MethodInfo? methodInfo)
    {
        if (methodInfo is null)
            return false;

        if (!methodInfo.IsVirtual)
            return false;

        Type objectType = methodInfo.ReflectedType!;
        bool isImplemented = methodInfo.DeclaringType == objectType;
        if (!isImplemented)
            return false;

        if (objectType.IsInterface && Api.IsSupportedGlobalSystemInterface(GetTypeDefName(objectType)))
        {
            return true;
        }

        foreach (var interfaceType in objectType.GetInterfaces())
        {
            var interfaceName = GetTypeDefName(interfaceType);
            bool isGlobalSystemInterface = Api.IsSupportedGlobalSystemInterface(interfaceName);
            if (!isGlobalSystemInterface)
                continue;

            if (objectType.IsInterface)
            {
                if (HasMethod(interfaceType, methodInfo))
                    return true;
            }
            else
            {
                var interfaceMap = objectType.GetInterfaceMap(interfaceType);
                if (interfaceMap.TargetMethods.Contains(methodInfo))
                    return true;
            }
        }
        return false;
    }

    private bool IsExtension(MethodInfo? methodInfo)
    {
        if (methodInfo is null)
            return false;

        return methodInfo.IsDefined(typeof(ExtensionAttribute));
    }

    private bool HasMethod(Type type, MethodInfo methodInfo)
    {
        var staticOrInstance = methodInfo.IsStatic ? BindingFlags.Static : BindingFlags.Instance;
        var name = GetMethodName(methodInfo);
        var parameterTypes = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
        var method = type.GetMethod(
            name,
            BindingFlags.Public | BindingFlags.ExactBinding | staticOrInstance,
            parameterTypes);
        return method is not null;
    }

    private bool HidesBaseField(FieldInfo? fieldInfo)
    {
        if (fieldInfo is null)
            return false;

        Type? baseType = fieldInfo.DeclaringType!.BaseType;
        if (baseType is null)
            return false;

        return HasField(baseType, fieldInfo);
    }

    private bool HasField(Type type, FieldInfo fieldInfo)
    {
        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
        {
            if (field.Name == fieldInfo.Name)
                return true;
        }

        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            if (field.Name == fieldInfo.Name)
                return true;
        }

        if (type.BaseType is not null)
        {
            return HasField(type.BaseType, fieldInfo);
        }
        else
        {
            return false;
        }
    }

    private Api.TypeDef? GetExplicitInterfaceType(MethodInfo? methodInfo)
    {
        if (methodInfo is null)
            return null;

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
                for (int i = 0; i < interfaceMap.InterfaceMethods.Length; ++i)
                {
                    if (interfaceMap.TargetMethods[i] == methodInfo && interfaceMap.InterfaceMethods[i].IsPublic)
                    {
                        return GetTypeDef(interfaceType);
                    }
                }
            }
        }

        return null;
    }

    private Api.ParameterDef GetParameterDef(ParameterInfo parameterInfo)
    {
        var type = parameterInfo.ParameterType;
        var typeDef = GetTypeDef(type);
        typeDef.IsNullable = ApiReflection.IsNullable(parameterInfo);
        typeDef.IsIn = parameterInfo.IsIn;
        typeDef.IsOut = parameterInfo.IsOut;

        return new Api.ParameterDef
        {
            Name = parameterInfo.Name,
            Type = typeDef,
        };
    }

    private Api.TypeDef GetTypeDef(Type type, Type[]? genericArgumentsOverride = null)
    {
        if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
        {
            // Use underlying type if System.Nullable as nullable is handled by TypeDef.IsNullable property.
            return GetTypeDef(Nullable.GetUnderlyingType(type)!, genericArgumentsOverride);
        }

        var name = GetTypeDefName(type);
        var typeDef = new Api.TypeDef
        {
            Name = name,
            IsPublic = IsPublicType(type),
            IsNullable = ApiReflection.IsNullable(type),
            IsAbstract = type.IsAbstract,
            IsSealed = type.IsSealed,
            IsEnum = type.IsEnum,
            IsClass = type.IsClass,
            IsArray = type.IsArray,
            IsDelegate = typeof(Delegate).IsAssignableFrom(type),
            IsPointer = type.IsPointer,
            IsFunctionPointer = type.IsFunctionPointer,
            IsByRef = type.IsByRef,
            IsGenericType = type.IsGenericType,
            IsGenericTypeParameter = type.IsGenericTypeParameter,
            IsGenericMethodParameter = type.IsGenericMethodParameter,
            GenericParameterPosition = type.IsGenericParameter ? type.GenericParameterPosition : 0,
            IsInterface = type.IsInterface,
            IsSystemObject = IsSystemType(name),
        };

        if (type.IsByRef || type.IsArray || type.IsPointer)
        {
            var elementType = type.GetElementType();
            typeDef.ElementType = GetTypeDef(elementType!);
        }
        else if (type.IsEnum)
        {
            AddEnum(type);
        }
        else if (!type.IsGenericParameter)
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

        if (type.IsGenericType)
        {
            var genericArgumentsCount = type.GetGenericArguments().Length;
            var genericArguments = genericArgumentsOverride ?? type.GetGenericArguments();
            for (int i = 0; i < genericArgumentsCount; ++i)
            {
                Type genericArgument = genericArguments[i];
                if (typeDef.GenericTypeArguments is null)
                {
                    typeDef.GenericTypeArguments = [];
                }
                typeDef.GenericTypeArguments.Add(GetTypeDef(genericArgument));
            }
        }

        if (type.IsNested && !type.IsGenericParameter)
        {
            typeDef.ParentType = GetTypeDef(type.DeclaringType!, type.GetGenericArguments());
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

        string name;
        if (type.IsNested)
        {
            name = type.Name;
        }
        else
        {
            name = type.ToString();
        }

        name = RemoveGenericExpression(name);
        name = RemovePointerExpression(name);
        name = ReplaceNestedTypeExpression(name);

        return name;
    }

    private string GetObjectTypeName(Type type)
    {
        return RemoveGenericExpression(type.Name);
    }

    private string GetTypeFullName(Type type)
    {
        var assembly = type.Assembly;
        var assemblyName = assembly.GetName().Name;

        if (type.FullName!.StartsWith("RpcUIShell.Core", StringComparison.Ordinal))
        {
            assemblyName = "RpcUIShell.Core";
        }
        else
        {
            assemblyName = assemblyName!.Replace(_exporterAssemblyName, _serverAssemblyName, StringComparison.Ordinal);
        }

        return $"{type.FullName}, {assemblyName}";
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

    private string ReplaceNestedTypeExpression(string name)
    {
        return name.Replace("+", ".", StringComparison.Ordinal);
    }

    private Api.EventDef GetEventDef(EventInfo eventInfo)
    {
        var eventDef = new Api.EventDef
        {
            Name = GetEventName(eventInfo),
            ExplicitInterfaceType = GetExplicitInterfaceType(eventInfo.AddMethod),
            IsVirtual = eventInfo.AddMethod?.IsVirtual ?? false,
            IsAbstract = eventInfo.AddMethod?.IsAbstract ?? false,
            IsOverride = IsOverride(eventInfo.AddMethod),
            HidesBase = HidesBaseEvent(eventInfo),
        };

        var invokeMethod = eventInfo.EventHandlerType!.GetMethod("Invoke");
        var parameters = invokeMethod!.GetParameters();
        foreach (var parameter in parameters)
        {
            if (eventDef.Parameters is null)
            {
                eventDef.Parameters = [];
            }
            eventDef.Parameters.Add(GetParameterDef(parameter));
        }
        return eventDef;
    }

    private string GetEventName(EventInfo eventInfo)
    {
        var name = eventInfo.Name;
        int dot = name.LastIndexOf('.');
        return name[(dot + 1)..];
    }

    private bool HidesBaseEvent(EventInfo? eventInfo)
    {
        if (eventInfo is null)
            return false;

        Type? baseType = eventInfo.DeclaringType!.BaseType;
        if (baseType is null)
            return false;

        return HasEvent(baseType, eventInfo);
    }

    private bool HasEvent(Type type, EventInfo eventInfo)
    {
        foreach (var e in type.GetEvents(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
        {
            if (e.Name == eventInfo.Name)
                return true;
        }

        foreach (var e in type.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            if (e.Name == eventInfo.Name)
                return true;
        }

        if (type.BaseType is not null)
        {
            return HasEvent(type.BaseType, eventInfo);
        }
        else
        {
            return false;
        }
    }

    private bool IsIgnoredNamespace(string? ns)
    {
        if (ns is null)
            return true;

        return ns.StartsWith("ABI.", StringComparison.Ordinal);
    }

    private bool IsIgnoredMethod(MethodInfo methodInfo)
    {
        bool isImplemented = methodInfo.DeclaringType == methodInfo.ReflectedType;
        bool isHiddenMethodsLikeGetterSetter = methodInfo.IsSpecialName;
        bool isCloneMethodForRecord = methodInfo.Name.Contains("<Clone>", StringComparison.Ordinal);
        return !isImplemented || isHiddenMethodsLikeGetterSetter || isCloneMethodForRecord;
    }

    private bool IsSystemType(string typeDefName)
    {
        return typeDefName.StartsWith("System.", StringComparison.Ordinal);
    }

    private void ExportToFile(string filePath)
    {
        var streamWriter = new StreamWriter(filePath, append: false, System.Text.Encoding.UTF8);
        var serializer = new XmlSerializer(typeof(Api));
        serializer.Serialize(streamWriter, Api);
        streamWriter.Close();
    }
}
