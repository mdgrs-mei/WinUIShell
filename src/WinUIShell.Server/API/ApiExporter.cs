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
    }

    private void AddObject(Type objectType)
    {
        var assembly = objectType.Assembly;
        var def = new Api.ObjectDef
        {
            Name = objectType.Name,
            FullName = $"{objectType.FullName}, {assembly.GetName().Name}",
            Namespace = objectType.Namespace!,
        };

        foreach (var propertyInfo in objectType.GetProperties(BindingFlags.Public | BindingFlags.Static))
        {
            def.StaticProperties.Add(GetPropertyDef(propertyInfo));
        }
        foreach (var propertyInfo in objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            def.InstanceProperties.Add(GetPropertyDef(propertyInfo));
        }

        foreach (var constructor in objectType.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
        {
            def.Constructors.Add(GetConstructorDef(constructor));
        }

        foreach (var staticMethod in objectType.GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            if (IsIgnoredMethod(staticMethod))
                continue;
            def.StaticMethods.Add(GetMethodDef(staticMethod));
        }
        foreach (var instanceMethod in objectType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
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
        var argumentType = new Api.ArgumentType
        {
            Name = propertyType.ToString(),
            IsNullable = Reflection.IsNullable(propertyInfo),
            IsEnum = propertyType.IsEnum,
            IsArray = propertyType.IsArray,
        };

        return new Api.PropertyDef
        {
            Name = propertyInfo.Name,
            Type = argumentType,
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
        var returnType = methodInfo.ReturnType;
        var returnParameter = methodInfo.ReturnParameter;

        var methodDef = new Api.MethodDef
        {
            Name = methodInfo.Name,
            ReturnType = new Api.ArgumentType
            {
                Name = returnType.ToString(),
                IsNullable = Reflection.IsNullable(returnParameter),
                IsEnum = returnType.IsEnum,
                IsArray = returnType.IsArray,
                IsByRef = returnType.IsByRef,
                IsIn = returnParameter.IsIn,
                IsOut = returnParameter.IsOut,
            },
            IsGenericMethod = methodInfo.IsGenericMethod
        };

        var parameters = methodInfo.GetParameters();
        foreach (var parameter in parameters)
        {
            methodDef.Parameters.Add(GetParameterDef(parameter));
        }
        return methodDef;
    }

    private Api.ParameterDef GetParameterDef(ParameterInfo parameterInfo)
    {
        var type = parameterInfo.ParameterType;
        var argumentType = new Api.ArgumentType
        {
            Name = type.ToString().Replace("&", "", StringComparison.Ordinal),
            IsNullable = Reflection.IsNullable(parameterInfo),
            IsEnum = type.IsEnum,
            IsArray = type.IsArray,
            IsByRef = type.IsByRef,
            IsIn = parameterInfo.IsIn,
            IsOut = parameterInfo.IsOut,
        };

        return new Api.ParameterDef
        {
            Name = parameterInfo.Name,
            Type = argumentType,
        };
    }

    private bool IsIgnoredMethod(MethodInfo methodInfo)
    {
        return methodInfo.IsSpecialName;
    }

    private void ExportToFile(string filePath)
    {
        var streamWriter = new StreamWriter(filePath, append: false, System.Text.Encoding.UTF8);
        var serializer = new XmlSerializer(typeof(Api));
        serializer.Serialize(streamWriter, _api);
        streamWriter.Close();
    }
}
