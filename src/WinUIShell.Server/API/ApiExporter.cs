using System.Reflection;
using System.Xml.Serialization;
using WinUIShell.Common;

namespace WinUIShell.Server;

public class ApiExporter : Singleton<ApiExporter>
{
    private readonly Api _api = new();

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
        AddObject(typeof(IEnumerator<>));
        AddObject(typeof(IDictionary<,>));
        AddObject(typeof(ICollection<>));
        AddObject(typeof(IList<>));
    }

    private void AddObject(Type type)
    {
        var assembly = type.Assembly;
        var def = new Api.ObjectDef
        {
            Name = GetObjectTypeName(type),
            FullName = $"{type.FullName}, {assembly.GetName().Name}",
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

        foreach (var staticMethod in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            if (IsIgnoredMethod(staticMethod))
                continue;
            def.StaticMethods.Add(GetMethodDef(staticMethod));
        }
        foreach (var instanceMethod in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
        {
            if (IsIgnoredMethod(instanceMethod))
                continue;
            def.InstanceMethods.Add(GetMethodDef(instanceMethod));
        }

        _api.Objects.Add(def);
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
            CanWrite = propertyInfo.CanWrite
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
            Name = methodInfo.Name,
            ReturnType = returnType,
            IsGenericMethod = methodInfo.IsGenericMethod,
            IsOverride = IsOverride(methodInfo),
        };

        var parameters = methodInfo.GetParameters();
        foreach (var parameter in parameters)
        {
            methodDef.Parameters.Add(GetParameterDef(parameter));
        }
        return methodDef;
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
            IsByRef = type.IsByRef,
            IsGenericType = type.IsGenericType,
            IsGenericTypeParameter = type.IsGenericTypeParameter,
            IsGenericMethodParameter = type.IsGenericMethodParameter,
            IsInterface = type.IsInterface,
        };

        if (type.IsByRef)
        {
            var elementType = type.GetElementType();
            typeDef.ElementType = GetTypeDef(elementType!);
        }
        else
        if (type.IsArray)
        {
            var elementType = type.GetElementType();
            typeDef.ElementType = GetTypeDef(elementType!);
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

        string name = type.ToString();
        name = RemoveGenericExpression(name);

        return name;
    }

    private string GetObjectTypeName(Type type)
    {
        return RemoveGenericExpression(type.Name);
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
