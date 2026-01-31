using System.Diagnostics;
using System.Reflection;
using System.Xml.Serialization;
using WinUIShell.Common;

namespace WinUIShell.Server;

public class ApiExporter : Singleton<ApiExporter>
{
    private readonly Api _api = new();
    private readonly HashSet<string> _addedEnums = [];
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

    private void AddEnum(Type type)
    {
        if (!type.IsEnum)
            return;

        var assembly = type.Assembly;
        string fullName = $"{type.FullName}, {assembly.GetName().Name}";

        if (_addedEnums.Contains(fullName))
            return;
        _ = _addedEnums.Add(fullName);

        var underlyingType = Enum.GetUnderlyingType(type);

        var def = new Api.EnumDef
        {
            Name = type.Name,
            FullName = fullName,
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

        _api.Enums.Add(def);
    }

    private void AddObjects()
    {
        AddObject(typeof(System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<>));
        AddObject(typeof(Microsoft.UI.Xaml.Controls.ContentDialog));
        AddObject(typeof(Microsoft.UI.Colors));
        AddObject(typeof(Microsoft.UI.Windowing.AppWindowTitleBar));
        AddObject(typeof(Microsoft.UI.Windowing.OverlappedPresenter));
        AddObject(typeof(Microsoft.UI.Windowing.CompactOverlayPresenter));
        AddObject(typeof(Microsoft.UI.Xaml.Controls.NavigationView));
        AddObject(typeof(Microsoft.UI.Xaml.Controls.Primitives.ButtonBase));
        AddObject(typeof(Microsoft.UI.Xaml.Controls.StackPanel));
        AddObject(typeof(Microsoft.UI.Xaml.Media.Animation.NavigationTransitionInfo));
        AddObject(typeof(Microsoft.UI.Xaml.Window));
        AddObject(typeof(Microsoft.UI.Xaml.Thickness));
        AddObject(typeof(Microsoft.UI.Xaml.Application));
        AddObject(typeof(Microsoft.UI.Xaml.DebugSettings));
        AddObject(typeof(Microsoft.UI.Xaml.ResourceDictionary));
        AddObject(typeof(Microsoft.UI.Xaml.Controls.Button));
        AddObject(typeof(Microsoft.UI.Xaml.Controls.TextBlock));
        AddObject(typeof(Microsoft.UI.Xaml.Controls.Frame));
        AddObject(typeof(Microsoft.UI.Xaml.Controls.FontIcon));
        AddObject(typeof(Microsoft.UI.Xaml.Controls.TabView));
        AddObject(typeof(Uri));
        AddObject(typeof(UriCreationOptions));
        AddObject(typeof(Windows.UI.Core.CoreDispatcher));
        AddObject(typeof(Microsoft.UI.Text.FontWeights));
        AddObject(typeof(Microsoft.UI.Xaml.Markup.XamlReader));
        AddObject(typeof(Microsoft.UI.Xaml.Controls.Page));
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

        foreach (var nestedType in type.GetNestedTypes())
        {
            if (def.NestedTypes is null)
            {
                def.NestedTypes = [];
            }
            def.NestedTypes.Add(GetObjectDef(nestedType));
        }

        return def;
    }

    private Api.PropertyDef GetPropertyDef(PropertyInfo propertyInfo)
    {
        var propertyType = propertyInfo.PropertyType;
        var typeDef = GetTypeDef(propertyType);
        typeDef.IsNullable = Reflection.IsNullable(propertyInfo);

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
            ImplementsSystemInterface = ImplementsSystemInterface(propertyInfo.GetMethod),
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
        typeDef.IsNullable = Reflection.IsNullable(fieldInfo);

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
            HidesBase = false,
            ImplementsSystemInterface = false,
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
        returnType.IsNullable = Reflection.IsNullable(returnParameter);
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
            HidesBase = HidesBaseMethod(methodInfo),
            ImplementsSystemInterface = ImplementsSystemInterface(methodInfo),
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

        return false;
    }

    private bool ImplementsSystemInterface(MethodInfo? methodInfo)
    {
        if (methodInfo is null)
            return false;

        if (!methodInfo.IsVirtual)
            return false;

        Type objectType = methodInfo.ReflectedType!;
        bool isImplemented = methodInfo.DeclaringType == objectType;
        if (!isImplemented)
            return false;

        if (objectType.IsInterface && IsSystemType(GetTypeDefName(objectType)))
        {
            return true;
        }

        foreach (var interfaceType in objectType.GetInterfaces())
        {
            var interfaceName = GetTypeDefName(interfaceType);
            bool isSystemInterface = IsSystemType(interfaceName);
            if (!isSystemInterface)
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
            IsAbstract = type.IsAbstract,
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
        else
        if (type.IsEnum)
        {
            AddEnum(type);
        }
        else
        if (!type.IsGenericParameter)
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
            foreach (var genericTypeArgument in type.GetGenericArguments())
            {
                if (typeDef.GenericTypeArguments is null)
                {
                    typeDef.GenericTypeArguments = [];
                }
                typeDef.GenericTypeArguments.Add(GetTypeDef(genericTypeArgument));
            }
        }

        if (type.IsNested && !type.IsGenericParameter)
        {
            typeDef.ParentType = GetTypeDef(type.DeclaringType!);
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
        if (type.IsNested)
        {
            return type.Name;
        }

        string name = type.ToString();
        name = RemoveGenericExpression(name);
        name = RemovePointerExpression(name);
        name = ReplaceNestedTypeExpression(name);

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

    private string ReplaceNestedTypeExpression(string name)
    {
        return name.Replace("+", ".", StringComparison.Ordinal);
    }

    private Api.EventDef GetEventDef(EventInfo eventInfo)
    {
        var eventDef = new Api.EventDef
        {
            Name = eventInfo.Name,
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

    private bool IsIgnoredMethod(MethodInfo methodInfo)
    {
        bool isImplemented = methodInfo.DeclaringType == methodInfo.ReflectedType;
        bool isHiddenMethodsLikeGetterSetter = methodInfo.IsSpecialName;
        return !isImplemented || isHiddenMethodsLikeGetterSetter;
    }

    private bool IsSystemType(string typeDefName)
    {
        return typeDefName.StartsWith("System.", StringComparison.Ordinal);
    }

    private void ExportToFile(string filePath)
    {
        var streamWriter = new StreamWriter(filePath, append: false, System.Text.Encoding.UTF8);
        var serializer = new XmlSerializer(typeof(Api));
        serializer.Serialize(streamWriter, _api);
        streamWriter.Close();
    }
}
