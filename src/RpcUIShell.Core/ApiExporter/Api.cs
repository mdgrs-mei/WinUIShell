using System.ComponentModel;

namespace RpcUIShell.Core;

#pragma warning disable CA1515 // Consider making public types internal
public class Api
{
#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1002 // Do not expose generic lists
#pragma warning disable CA2227 // Collection properties should be read only

    public static readonly List<(string FullName, string ShortName)> SystemTypes =
    [
        ("System.Boolean", "bool"),
        ("System.Byte", "byte"),
        ("System.SByte", "sbyte"),
        ("System.Char", "char"),
        ("System.Decimal", "decimal"),
        ("System.Double", "double"),
        ("System.Single", "float"),
        ("System.Int32", "int"),
        ("System.UInt32", "uint"),
        ("System.Int64", "long"),
        ("System.UInt64", "ulong"),
        ("System.Int16", "short"),
        ("System.UInt16", "ushort"),
        ("System.String", "string"),
        ("System.Object", "object"),
        ("System.Void", "void"),
    ];

    public string ModuleName { get; set; } = "";
    public string ServerName { get; set; } = "";
    public string ObjectInterfaceName
    {
        get => $"I{ModuleName}Object";
    }
    public string ObjectIdMemberName
    {
        get => $"{ModuleName}ObjectId";
    }

    public List<string> UnsupportedNamespaces { get; set; } = [];
    public List<string> UnsupportedTypes { get; set; } = [];
    public List<string> SupportedGlobalSystemInterfaces { get; set; } = [];
    public List<string> EmulatedSystemInterfaces { get; set; } = [];
    public List<string> UnsupportedMethodNames { get; set; } = [];
    public List<TypeMappingDef> TypeMappings { get; } = [];
    public List<EnumDef> Enums { get; } = [];
    public List<ObjectDef> Objects { get; } = [];

    public class TypeMappingDef
    {
        public string Name { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Namespace { get; set; } = "";

        [DefaultValue(0)]
        public int GenericArgumentCount { get; set; }
    }

    public class EnumDef
    {
        public string Name { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Namespace { get; set; } = "";
        public string UnderlyingType { get; set; } = "";
        public List<EnumEntryDef> Entries { get; } = [];
    }

    public class EnumEntryDef
    {
        public string Name { get; set; } = "";
        public object? Value { get; set; }
    }

    public class ObjectDef
    {
        public string Name { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Namespace { get; set; } = "";
        public TypeDef Type { get; set; } = new();

        [DefaultValue(null)]
        public TypeDef? BaseType { get; set; }

        [DefaultValue(null)]
        public List<TypeDef>? Interfaces { get; set; }

        [DefaultValue(null)]
        public List<PropertyDef>? StaticProperties { get; set; }

        [DefaultValue(null)]
        public List<PropertyDef>? InstanceProperties { get; set; }

        [DefaultValue(null)]
        public List<MethodDef>? Constructors { get; set; }

        [DefaultValue(null)]
        public List<MethodDef>? StaticMethods { get; set; }

        [DefaultValue(null)]
        public List<MethodDef>? InstanceMethods { get; set; }

        [DefaultValue(null)]
        public List<EventDef>? StaticEvents { get; set; }

        [DefaultValue(null)]
        public List<EventDef>? InstanceEvents { get; set; }

        [DefaultValue(null)]
        public List<ObjectDef>? NestedObjects { get; set; }

        [DefaultValue(null)]
        public List<EnumDef>? NestedEnums { get; set; }
    }

    public class PropertyDef
    {
        public string Name { get; set; } = "";
        public TypeDef Type { get; set; } = new();

        [DefaultValue(null)]
        public TypeDef? ExplicitInterfaceType { get; set; }

        [DefaultValue(null)]
        public List<ParameterDef>? IndexParameters { get; set; }

        [DefaultValue(false)]
        public bool CanRead { get; set; }

        [DefaultValue(false)]
        public bool CanWrite { get; set; }

        [DefaultValue(false)]
        public bool IsVirtual { get; set; }

        [DefaultValue(false)]
        public bool IsAbstract { get; set; }

        [DefaultValue(false)]
        public bool IsOverride { get; set; }

        [DefaultValue(false)]
        public bool HidesBase { get; set; }

        [DefaultValue(false)]
        public bool ImplementsInterface { get; set; }

        [DefaultValue(false)]
        public bool ImplementsGlobalSystemInterface { get; set; }
    }

    public class TypeDef
    {
        public string Name { get; set; } = "";

        [DefaultValue(true)]
        public bool IsPublic { get; set; } = true;

        [DefaultValue(false)]
        public bool IsNullable { get; set; }

        [DefaultValue(false)]
        public bool IsAbstract { get; set; }

        [DefaultValue(false)]
        public bool IsSealed { get; set; }

        [DefaultValue(false)]
        public bool IsEnum { get; set; }

        [DefaultValue(true)]
        public bool IsClass { get; set; } = true;

        [DefaultValue(false)]
        public bool IsArray { get; set; }

        [DefaultValue(false)]
        public bool IsDelegate { get; set; }

        [DefaultValue(false)]
        public bool IsPointer { get; set; }

        [DefaultValue(false)]
        public bool IsFunctionPointer { get; set; }

        [DefaultValue(false)]
        public bool IsByRef { get; set; }

        [DefaultValue(false)]
        public bool IsIn { get; set; }

        [DefaultValue(false)]
        public bool IsOut { get; set; }

        [DefaultValue(false)]
        public bool IsGenericType { get; set; }

        [DefaultValue(false)]
        public bool IsGenericTypeParameter { get; set; }

        [DefaultValue(false)]
        public bool IsGenericMethodParameter { get; set; }

        [DefaultValue(0)]
        public int GenericParameterPosition { get; set; }

        [DefaultValue(false)]
        public bool IsInterface { get; set; }

        [DefaultValue(false)]
        public bool IsSystemObject { get; set; }

        [DefaultValue(null)]
        public TypeDef? ElementType { get; set; }

        // Parent type if this is a nested type.
        [DefaultValue(null)]
        public TypeDef? ParentType { get; set; }

        [DefaultValue(null)]
        public List<TypeDef>? GenericTypeArguments { get; set; }
    }

    public class MethodDef
    {
        public string? Name { get; set; }

        [DefaultValue(null)]
        public TypeDef? ReturnType { get; set; }

        [DefaultValue(null)]
        public List<ParameterDef>? Parameters { get; set; }

        [DefaultValue(null)]
        public TypeDef? ExplicitInterfaceType { get; set; }

        [DefaultValue(false)]
        public bool IsGenericMethod { get; set; }

        [DefaultValue(false)]
        public bool IsVirtual { get; set; }

        [DefaultValue(false)]
        public bool IsAbstract { get; set; }

        [DefaultValue(false)]
        public bool IsOverride { get; set; }

        [DefaultValue(false)]
        public bool IsExtension { get; set; }

        [DefaultValue(false)]
        public bool HidesBase { get; set; }

        [DefaultValue(false)]
        public bool ImplementsInterface { get; set; }

        [DefaultValue(false)]
        public bool ImplementsGlobalSystemInterface { get; set; }
    }

    public class EventDef
    {
        public string Name { get; set; } = "";

        [DefaultValue(null)]
        public List<ParameterDef>? Parameters { get; set; }

        [DefaultValue(null)]
        public TypeDef? ExplicitInterfaceType { get; set; }

        [DefaultValue(false)]
        public bool IsVirtual { get; set; }

        [DefaultValue(false)]
        public bool IsAbstract { get; set; }

        [DefaultValue(false)]
        public bool IsOverride { get; set; }

        [DefaultValue(false)]
        public bool IsExtension { get; set; }

        [DefaultValue(false)]
        public bool HidesBase { get; set; }
    }

    public class ParameterDef
    {
        public string? Name { get; set; } = "";
        public TypeDef Type { get; set; } = new();
    }

    public static bool TryReplaceSystemTypeNameWithShortName(string fullTypeName, out string? shortTypeName)
    {
        foreach (var (fullName, shortName) in SystemTypes)
        {
            if (fullTypeName == fullName)
            {
                shortTypeName = shortName;
                return true;
            }
        }
        shortTypeName = null;
        return false;
    }

    public bool IsUnsupportedType(string typeDefName)
    {
        if (typeDefName is null)
            return true;

        if (UnsupportedTypes.Contains(typeDefName))
            return true;

        foreach (string unsupportedNamespace in UnsupportedNamespaces)
        {
            if (typeDefName.StartsWith($"{unsupportedNamespace}.", StringComparison.Ordinal))
                return true;
        }
        return false;
    }

    public bool IsSupportedGlobalSystemInterface(string typeDefName)
    {
        return SupportedGlobalSystemInterfaces.Contains(typeDefName);
    }

    public bool IsSupportedSystemInterface(string typeDefName)
    {
        return IsSupportedGlobalSystemInterface(typeDefName) || EmulatedSystemInterfaces.Contains(typeDefName);
    }

    public bool IsUnsupportedMethod(string methodName)
    {
        return UnsupportedMethodNames.Contains(methodName);
    }
#pragma warning restore CA2227
#pragma warning restore CA1002
#pragma warning restore CA1034
}
#pragma warning restore CA1515
